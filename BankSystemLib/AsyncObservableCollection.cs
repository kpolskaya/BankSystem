using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankSystemLib
{
    /// <summary>
    /// Асинхронная версия OservableCollection, которая не приводит к исключению, если коллекция изменяется
    /// не в том потоке, который ее создал.
    /// </summary>
    internal class AsyncObservableCollection<T> : ObservableCollection<T> //не пригодился тут но пусть полежит
    {
        private SynchronizationContext synchronizationContext = SynchronizationContext.Current;

        public AsyncObservableCollection()
        {

        }

        public AsyncObservableCollection(IEnumerable<T> list)
            : base(list)
        {

        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                RaiseCollectionChanged(e); //поднимаем событие в текущем потоке
            }
            else synchronizationContext.Send(RaiseCollectionChanged, e); //поднимаем событие в потоке создателя коллекции
          
        }

        private void RaiseCollectionChanged(object p)
        {
            base.OnCollectionChanged((NotifyCollectionChangedEventArgs)p);

        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (SynchronizationContext.Current == synchronizationContext)
            {
                RaisePropertyChanged(e);
            }
            else synchronizationContext.Send(RaisePropertyChanged, e);
            
        }

        private void RaisePropertyChanged(object p)
        {
            base.OnPropertyChanged((PropertyChangedEventArgs)p); 
        }
    }
}
