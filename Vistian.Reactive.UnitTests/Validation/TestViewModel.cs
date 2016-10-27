using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.UnitTests.Validation
{
    public class TestViewModel : ReactiveObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private string _name2;

        public string Name2
        {
            get { return _name2; }
            set { this.RaiseAndSetIfChanged(ref _name2, value); }
        }

        public List<string> GetIt { get; set; } = new List<string>();

        public string Go()
        {
            return "here";
        }
    }
}
