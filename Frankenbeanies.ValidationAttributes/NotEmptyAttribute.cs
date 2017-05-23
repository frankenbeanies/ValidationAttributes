﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frankenbeanies.ValidationAttributes
{
  /// <summary>
  /// Determines whether or not a list is empty
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
  public class NotEmptyAttribute : ValidationAttribute
    {
#if SILVERLIGHT
        internal
#else
        public
#endif
        override bool IsValid(object values)
        {
            if (values == null) return true; //checking if values has been set is the job of the RequiredAttribute, not this one. 

            if (!(values is ICollection)) throw new InvalidOperationException("NotEmptyAttribute requires an ICollection");

            if ((values as ICollection).Count == 0) return false;

            return true;
        }
    }
}
