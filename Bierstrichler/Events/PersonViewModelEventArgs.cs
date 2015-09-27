using Bierstrichler.ViewModels.Persons;
using GalaSoft.MvvmLight;
using System;

namespace Bierstrichler.Events
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class PersonViewModelEventArgs : EventArgs
    {
        public PersonViewModel Person { get; set; }

        public int Index { get; set; }

        public PersonViewModelEventArgs(PersonViewModel person = null)
        {
            this.Person = person;
            Index = -1;
        }

        public PersonViewModelEventArgs(PersonViewModel person, int index)
        {
            this.Person = person;
            Index = index;
        }
    }
}