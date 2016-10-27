﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Validation
{
    public static class ValidationContextMixins
    {
        /// <summary>
        /// Resolve the property valuation for a specified property
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TProperty1"></typeparam>
        /// <returns></returns>
        public static BasePropertyValidation<TViewModel, TProperty1> ResolveFor<TViewModel, TProperty1>(this ValidationContext context, Expression<Func<TViewModel, TProperty1>> viewModelProperty1, bool strict = true)
        {
            var instance = context.Validations.
                Where(p => p is BasePropertyValidation<TViewModel, TProperty1>).
                Cast<BasePropertyValidation<TViewModel, TProperty1>>().FirstOrDefault(v => v.ContainsProperty(viewModelProperty1, strict));

            return instance;
        }

        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2> ResolveFor<TViewModel, TProperty1, TProperty2>(this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1, Expression<Func<TViewModel, TProperty1>> viewModelProperty2, bool strict = true)
        {
            var instance = context.Validations.
                Where(p => p is BasePropertyValidation<TViewModel, TProperty1, TProperty2>).
                Cast<BasePropertyValidation<TViewModel, TProperty1, TProperty2>>().FirstOrDefault(v => v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2) && v.PropertyCount == 2);

            return instance;
        }

        public static BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3> ResolveFor<TViewModel, TProperty1, TProperty2, TProperty3>(this ValidationContext context,
            Expression<Func<TViewModel, TProperty1>> viewModelProperty1, Expression<Func<TViewModel, TProperty1>> viewModelProperty2, Expression<Func<TViewModel, TProperty1>> viewModelProperty3, bool strict = true)
        {
            var instance = context.Validations.
                Where(p => p is BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>).
                Cast<BasePropertyValidation<TViewModel, TProperty1, TProperty2, TProperty3>>().FirstOrDefault(v => v.ContainsProperty(viewModelProperty1) && v.ContainsProperty(viewModelProperty2) && v.ContainsProperty(viewModelProperty3) && v.PropertyCount == 3);

            return instance;
        }

    }
}
