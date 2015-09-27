using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bierstrichler.Data.Questions;

namespace Bierstrichler.ViewModels.Questions
{
    public class PairViewModel : ViewModelBase
    {
        public PairViewModel(Pair<string, bool> model)
        {
            Model = model;
            PropertyChanged += PairViewModel_PropertyChanged;
        }

        void PairViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            App.SaveQuestionsAsync();
        }

        public Pair<string, bool> Model {set; get;}

        public string First
        {
            get { return Model.First; }
            set
            {
                Model.First = value;
                RaisePropertyChanged();
            }
        }

        public bool Second
        {
            get { return Model.Second; }
            set
            {
                Model.Second = value;
                RaisePropertyChanged();
            }
        }

    }
}
