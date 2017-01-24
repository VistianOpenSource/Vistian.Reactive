# Vistian.Reactive.Validation

Validation for ReactiveUI based solutions, functioning in a reactive way.

## How to use

* For those View models which need validation, implement ISupportsValidation
* Add validation rules to the view model
* Bind to the validation rules in the view

## Example
Decorate existing ViewModel with ISupportsValidation, which has a single member, ValidationContext. The ValidationContext contains all of the functionality surrounding the validation of the view model.  Most access to the specification of validation rules is performed through extension methods on the ISupportsValidation interface.

``` 
public class SampleViewModel:ReactiveObject,ISupportsValidation
{
    public ValidationContext ValidationContext { get; } = new ValidationContext();

```

Add validation to the view model

```
    // Bindable rule
    public ValidationHelper ComplexRule { get; set; }

    // bindable rule
    public ValidationHelper AgeRule { get; set; }

    public SampleViewModel()
    {
    // name must be at least 3 chars - the selector here is the property name and its a single property validator

    this.ValidationRule(vm => vm.Name, _isDefined, "You must specify a valid name");

    // age must be between 13 and 100, message includes the silly length being passed in, store in a property of the view model

    AgeRule = this.ValidationRule(vm => vm.Age, age => age >= 13 && age <= 100,(a) => $"{a} is a silly age");

    // create a rule using an observable. Store in a property of the view model

    ComplexRule = this.ValidationRule(_ => this.WhenAny(vm => vm.Age, vm => vm.Name, (a, n) => new { Age = a.Value, Name = n.Value }).Select(v => v.Age > 10 && !string.IsNullOrEmpty(v.Name)),
    (vm,state) => (!state) ? "Thats a ridiculous name / age combination" : string.Empty);

    // Save command only active when all validators are valid
    Save = ReactiveCommand.CreateAsyncTask(this.IsValid(), async _ =>
    {
    });
}

```

Add validation presentation to the view.

```
public class MainActivity : ReactiveAppCompatActivity<SampleViewModel>
{
    public EditText nameEdit { get; set; }

    public EditText ageEdit { get; set; }

    public TextView nameValidation { get; set; }

    public TextView ageValidation { get; set; }

    public TextView validationSummary { get; set; }

    public Button myButton { get; set; }

    public TextInputLayout til { get; set; }

    public TextInputEditText tiet { get; set; }

    protected override void OnCreate (Bundle bundle)
    {
        base.OnCreate (bundle);

        // Set our view from the "main" layout resource
        SetContentView (Resource.Layout.Main);

        WireUpControls();

        this.BindCommand(ViewModel, vm => vm.Save, v => v.myButton);

        this.Bind(ViewModel, vm => vm.Name, v => v.nameEdit.Text);

        this.Bind(ViewModel, vm => vm.Age, v => v.ageEdit.Text);

        // Bind any validations which reference the name property to the text of the nameValidation control
        this.BindValidation(ViewModel, vm => vm.Name, v => v.nameValidation.Text);

        // Bind the validation specified by the AgeRule to the text of the ageValidation control
        this.BindValidation(ViewModel, vm => vm.AgeRule, v => v.ageValidation.Text);

        // bind the summary validation text to the validationSummary control
        this.BindValidation(ViewModel, v => v.validationSummary.Text);

        // bind to an Android TextInputLayout control, utilising the Error property
        this.BindValidation(ViewModel, vm => vm.ComplexRule,til);
    }
}

```

## Capabilities

* Rules can be composed of single or multiple properties along along with more generic Observables.
* Validation text can encapsulate both valid and invalid states.
* Binding can occur to either a view or an action.
* Validation text can reference either the view model or properties which comprise the validation rule e.g. include text entered as part of validation message.
* Validation text output can be adjusted using custom formatters, not only allowing for single & multiline output but also for platforms like Android it should be possible to achieve richer renderings i.e. Bold/italics.
* Validation rules in the view model "should" allow for binding of validation results for those environments that support binding (XAML). It should be noted that this however has not been tested.

## Outstanding

* Binding hasn't been tried at all with Xamarin Forms.
* An Android specific binding for TextInputLayout has been created, but no platform specific bindings for iOS have been created.
* Significant usage!
* Possible additional bit of work related to validationContext & binding.
* Possible tidyup regarding setup of bindable ValidationHelper in view model.

## How it Works
In essence, its a relatively simple model of the ValidationContext containing a list of IValidationComponent instances. 

An IValidationComponent provides an observable for ValidationState. Whenever validation state changes (either a transition of validity) or ValidationText changes, then a new value is pushed out.


