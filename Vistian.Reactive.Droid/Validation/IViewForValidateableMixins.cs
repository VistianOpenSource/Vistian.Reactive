using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using Vistian.Reactive.Validation;

namespace Vistian.Reactive.Droid.Validation
{
    public static class IViewForValidateableMixins
    {
        /// <summary>
        /// Platform binding to the <see cref="TextInputLayout"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProp"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation<TView, TViewModel, TViewModelProp>(this TView view,
            TViewModel viewModel, Expression<Func<TViewModel, TViewModelProp>> viewModelProperty,
            TextInputLayout viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForProperty(view, viewModelProperty, (ValidationState v, string f) => viewProperty.Error = f, SingleLineFormatter.Default);
        }


        /// <summary>
        /// Platform binding to the <see cref="TextInputLayout"/>
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModel"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="viewProperty"></param>
        /// <returns></returns>
        public static IDisposable BindValidation<TView, TViewModel>(this TView view,
            TViewModel viewModel, Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            TextInputLayout viewProperty)
            where TViewModel : ReactiveObject, ISupportsValidation
            where TView : IViewFor<TViewModel>
        {
            return ValidationBinding.ForValidationHelperProperty(view, viewModelHelperProperty, (ValidationState v, string f) => viewProperty.Error = f, SingleLineFormatter.Default);
        }


    }
}