using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Providers;

namespace Vistian.Reactive.Validation
{
    /// <summary>
    /// A validation binding.
    /// </summary>
    public class ValidationBinding : IValidationBinding
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        /// <summary>
        /// Create an instance with a specified observable for validation changes.
        /// </summary>
        /// <param name="validationObservable"></param>
        public ValidationBinding(IObservable<Unit> validationObservable)
        {
            _disposables.Add(validationObservable.Subscribe());
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

        /// <summary>
        /// Create a binding between a view model property and a view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty1"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="viewProperty"></param>
        /// <param name="formatter"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty1, TViewProperty>(TView view,
            Expression<Func<TViewModel, TViewModelProperty1>> viewModelProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string> formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                Select(
                    viewModel =>
                        viewModel.ValidationContext.ResolveFor<TViewModel, TViewModelProperty1>(viewModelProperty, strict)
                            .ValidationStatusChange).
                Switch().
                Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty).
                Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Binding a specified view model property to a provided action.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewModelProperty1"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModelProperty"></param>
        /// <param name="action"></param>
        /// <param name="formatter"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        public static IValidationBinding ForProperty<TView, TViewModel, TViewModelProperty1, TOut>(TView view,
            Expression<Func<TViewModel, TViewModelProperty1>> viewModelProperty,
            Action<ValidationState, TOut> action,
            IValidationTextFormatter<TOut> formatter = null,
            bool strict = true)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                Select(
                    viewModel =>
                        viewModel.ValidationContext.ResolveFor(viewModelProperty, strict)
                            .ValidationStatusChange).
                Switch().
                Select(vc => new { ValidationChange = vc, Formatted = formatter.Format(vc.Text) }).
                Do(r => action(r.ValidationChange, r.Formatted)).
                Select(_ => Unit.Default);

            return new ValidationBinding(vcObs);

        }

        /// <summary>
        /// Create a binding between a <see cref="ValidationHelper"/> and a specified view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="viewProperty"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TViewProperty>(TView view,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string> formatter = null)
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation

        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                Select(
                    viewModel =>
                            viewModel.WhenAnyValue(viewModelHelperProperty).SelectMany(vy => vy.ValidationChanged)).
                Switch().
                Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty).
                Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Bind a <see cref="ValidationHelper"/> to a specified action.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewModelHelperProperty"></param>
        /// <param name="action"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static IValidationBinding ForValidationHelperProperty<TView, TViewModel, TOut>(TView view,
            Expression<Func<TViewModel, ValidationHelper>> viewModelHelperProperty,
            Action<ValidationState, TOut> action,
            IValidationTextFormatter<TOut> formatter = null
            )
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                Select(
                    viewModel =>
                            viewModel.WhenAnyValue(viewModelHelperProperty).SelectMany(vy => vy.ValidationChanged)).
                Switch().
                Select(vc => new { ValidationChange = vc, Formatted = formatter.Format(vc.Text) });

            var updateObs = vcObs.Do(r =>
            {
                action(r.ValidationChange, r.Formatted);
            }).
                Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Create a binding between a view model and a specified action.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="view"></param>
        /// <param name="action"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static IValidationBinding ForViewModel<TView, TViewModel, TOut>(TView view,
            Action<TOut> action,
            IValidationTextFormatter<TOut> formatter
            )
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                Select(
                    viewModel =>
                            viewModel.ValidationContext.Text).Select(formatter.Format);

            var updateObs = vcObs.Do(action).
                Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }

        /// <summary>
        /// Create a binding between a view model and a view property.
        /// </summary>
        /// <typeparam name="TView"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TViewProperty"></typeparam>
        /// <param name="view"></param>
        /// <param name="viewProperty"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static IValidationBinding ForViewModel<TView, TViewModel, TViewProperty>(TView view,
            Expression<Func<TView, TViewProperty>> viewProperty,
            IValidationTextFormatter<string> formatter = null
            )
            where TView : IViewFor<TViewModel>
            where TViewModel : ReactiveObject, ISupportsValidation
        {
            if (formatter == null)
            {
                formatter = SingleLineFormatter.Default;
            }

            var vcObs = view.WhenAnyValue(v => v.ViewModel).
                Where(vm => vm != null).
                SelectMany(
                    viewModel =>
                            viewModel.ValidationContext.ValidationStatusChange).
                            Select(vc => formatter.Format(vc.Text));

            var updateObs = BindToView(vcObs, view, viewProperty).
                            Select(_ => Unit.Default);

            return new ValidationBinding(updateObs);
        }


        public static IObservable<TValue> BindToView<TView, TViewProp, TTarget, TValue>(IObservable<TValue> valueChange, TTarget target, Expression<Func<TView, TViewProp>> viewProperty)
        {
            var viewExpression = global::ReactiveUI.Reflection.Rewrite(viewProperty.Body);

            var setter = global::ReactiveUI.Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                return valueChange.Do(
                    x => setter(target, x, viewExpression.GetArgumentsArray()),
                    ex =>
                    {
                        RxLog.Log(typeof(ValidationBinding), Classified.Error(ex));
                        //todo: this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    });
            }

            var bindInfo = valueChange.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Do(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex =>
                    {
                        RxLog.Log(typeof(ValidationBinding), Classified.Error(ex));
                        //todo: this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    })
                .Select(v => v.val);
        }

    }
}
