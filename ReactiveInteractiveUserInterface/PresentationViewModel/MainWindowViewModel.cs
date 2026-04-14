//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor
        public MainWindowViewModel() : this(null) { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x =>
            {
                Balls.Add(x);
                RaisePropertyChanged(nameof(BallCount));
                RemoveBallCommand?.RaiseCanExecuteChanged();
            });

            AddBallCommand = new RelayCommand(AddBall);
            RemoveBallCommand = new RelayCommand(RemoveBall, () => BallCount > 0);
        }
        #endregion ctor

        #region public API

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();
        public int BallCount => Balls.Count;

        public RelayCommand AddBallCommand { get; }
        public RelayCommand RemoveBallCommand { get; }

        public void Start(int numberOfBalls)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
        }

        private void AddBall()
        {
            if (Disposed) return;
            ModelLayer.AddBall();
        }

        private void RemoveBall()
        {
            if (Disposed) return;
            
            ModelLayer.RemoveLastBall();

            if (Balls.Count > 0)
            {
                Balls.RemoveAt(Balls.Count - 1);
                
                RaisePropertyChanged(nameof(BallCount));
                RemoveBallCommand?.RaiseCanExecuteChanged();
            }
        }
        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}