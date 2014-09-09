using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using ActionDelegate = System.Action;

namespace Caliburn.Micro
{
    public class XamarinPlatformProvider : IPlatformProvider
    {

        public XamarinPlatformProvider()
        {
        }

        public void BeginOnUIThread(ActionDelegate action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

        public Task OnUIThreadAsync(ActionDelegate action)
        {
            return Task.Factory.StartNew(() => Device.BeginInvokeOnMainThread(action));
        }

        public void OnUIThread(ActionDelegate action) {
            Device.BeginInvokeOnMainThread(action);
        }

        public object GetFirstNonGeneratedView(object view)
        {
            return view;
        }

        public void ExecuteOnFirstLoad(object view, Action<object> handler)
        {
            handler(view);
        }

        public void ExecuteOnLayoutUpdated(object view, Action<object> handler)
        {
            handler(view);
        }

        public ActionDelegate GetViewCloseAction(object viewModel, ICollection<object> views, bool? dialogResult)
        {
            throw new NotImplementedException();
        }

        public bool InDesignMode
        {
            get { return true; }
        }
    }
}