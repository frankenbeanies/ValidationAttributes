using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankenbeanies.ValidationAttributes
{
    /// <summary>
    /// Used for specifying a range constraint on a list
    /// Derived from System.ComponentModel.DataAnnotations.RangeAttribute as described at 
    /// https://github.com/Microsoft/referencesource/blob/90b323fe52bec428fe4bd5f007e9ead6b265d553/System.ComponentModel.DataAnnotations/DataAnnotations/RangeAttribute.cs
    /// in accordance with the MIT License as described at
    /// https://github.com/Microsoft/referencesource/blob/90b323fe52bec428fe4bd5f007e9ead6b265d553/LICENSE.txt
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "We want it to be accessible via method on parent.")]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class ListRangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// Gets the minimum value for the range
        /// </summary>
        public object Minimum { get; private set; }

        /// <summary>
        /// Gets the maximum value for the range
        /// </summary>
        public object Maximum { get; private set; }

        /// <summary>
        /// Gets the type of the <see cref="Minimum"/> and <see cref="Maximum"/> values (e.g. Int32, Double, or some custom type)
        /// </summary>
        public Type OperandType { get; private set; }

        protected Func<object, object> Conversion { get; set; }

        /// <summary>
        /// Constructor that takes integer minimum and maximum values
        /// </summary>
        /// <param name="minimum">The minimum value, inclusive</param>
        /// <param name="maximum">The maximum value, inclusive</param>
        public ListRangeAttribute(int minimum, int maximum)
            : this()
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.OperandType = typeof(int);
        }

        /// <summary>
        /// Constructor that takes double minimum and maximum values
        /// </summary>
        /// <param name="minimum">The minimum value, inclusive</param>
        /// <param name="maximum">The maximum value, inclusive</param>
        public ListRangeAttribute(double minimum, double maximum)
            : this()
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.OperandType = typeof(double);
        }

        /// <summary>
        /// Allows for specifying range for arbitrary types. The minimum and maximum strings will be converted to the target type.
        /// </summary>
        /// <param name="type">The type of the range parameters. Must implement IComparable.</param>
        /// <param name="minimum">The minimum allowable value.</param>
        /// <param name="maximum">The maximum allowable value.</param>
        public ListRangeAttribute(Type type, string minimum, string maximum)
            : this()
        {
            this.OperandType = type;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        private ListRangeAttribute()
            : base(() => "There was an error validating the ListRangeAttribute")
        {
        }

        private void Initialize(IComparable minimum, IComparable maximum, Func<object, object> conversion)
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new InvalidOperationException($"Min({minimum}) is greater than Max({maximum})");
            }

            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Conversion = conversion;
        }

        /// <summary>
        /// Returns true if the value falls between min and max, inclusive.
        /// </summary>
        /// <param name="value">The value to test for validity.</param>
        /// <returns><c>true</c> means the <paramref name="value"/> is valid</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
#if !SILVERLIGHT
        public
#else
        internal
#endif
        override bool IsValid(object values)
        {
            // Validate our properties and create the conversion function
            this.SetupConversion();

            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            if (values == null)
            {
                return true;
            }

            if (!(values is IEnumerable)) throw new InvalidOperationException("Object is not an IEnumerable.");

            foreach(var value in (IEnumerable)values)
            {
                string s = value as string;
                if (s != null && String.IsNullOrEmpty(s))
                {
                    continue;
                }

                object convertedValue = null;

                try
                {
                    convertedValue = this.Conversion(value);
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }

                IComparable min = (IComparable)this.Minimum;
                IComparable max = (IComparable)this.Maximum;
                if(!(min.CompareTo(convertedValue) <= 0 && max.CompareTo(convertedValue) >= 0)) return false;
            }

            return true;
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.FormatErrorMessage"/>
        /// </summary>
        /// <remarks>This override exists to provide a formatted message describing the minimum and maximum values</remarks>
        /// <param name="name">The user-visible name to include in the formatted message.</param>
        /// <returns>A localized string describing the minimum and maximum values</returns>
        /// <exception cref="InvalidOperationException"> is thrown if the current attribute is ill-formed.</exception>
        public override string FormatErrorMessage(string name)
        {
            this.SetupConversion();

            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, this.Minimum, this.Maximum);
        }

        /// <summary>
        /// Validates the properties of this attribute and sets up the conversion function.
        /// This method throws exceptions if the attribute is not configured properly.
        /// If it has once determined it is properly configured, it is a NOP.
        /// </summary>
        private void SetupConversion()
        {
            if (this.Conversion == null)
            {
                object minimum = this.Minimum;
                object maximum = this.Maximum;

                if (minimum == null || maximum == null)
                {
                    throw new InvalidOperationException("ListRangeAttribute must set both min and max");
                }

                // Careful here -- OperandType could be int or double if they used the long form of the ctor.
                // But the min and max would still be strings.  Do use the type of the min/max operands to condition
                // the following code.
                Type operandType = minimum.GetType();

                if (operandType == typeof(int))
                {
                    this.Initialize((int)minimum, (int)maximum, v => Convert.ToInt32(v, CultureInfo.InvariantCulture));
                }
                else if (operandType == typeof(double))
                {
                    this.Initialize((double)minimum, (double)maximum, v => Convert.ToDouble(v, CultureInfo.InvariantCulture));
                }
                else
                {
                    Type type = this.OperandType;
                    if (type == null)
                    {
                        throw new InvalidOperationException("ListRangeAttribute must provide Operand Type");
                    }
                    Type comparableType = typeof(IComparable);
                    if (!comparableType.IsAssignableFrom(type))
                    {
                        throw new InvalidOperationException(
                            String.Format(
                                CultureInfo.CurrentCulture,
                                "The type is not comparable.",
                                type.FullName,
                                comparableType.FullName));
                    }

#if SILVERLIGHT
                    Func<object, object> conversion = value => (value != null && value.GetType() == type) ? value : Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    IComparable min = (IComparable)conversion(minimum);
                    IComparable max = (IComparable)conversion(maximum);
#else
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    IComparable min = (IComparable)converter.ConvertFromString((string)minimum);
                    IComparable max = (IComparable)converter.ConvertFromString((string)maximum);

                    Func<object, object> conversion = value => (value != null && value.GetType() == type) ? value : converter.ConvertFrom(value);
#endif // !SILVERLIGHT

                    this.Initialize(min, max, conversion);
                }
            }
        }
    }
}
