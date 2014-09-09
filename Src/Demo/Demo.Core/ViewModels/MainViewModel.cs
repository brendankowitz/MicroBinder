using System;
using Caliburn.Micro;
using MicroBinder.Events;

namespace Demo.Core.ViewModels
{
    public class MainViewModel : Screen
    {
        public MainViewModel ()
        {
           
        }

        public void ButtonClicked(object button)
        {
            var a = button;
        }
    }
}

