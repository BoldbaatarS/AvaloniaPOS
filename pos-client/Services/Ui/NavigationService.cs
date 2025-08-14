using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace RestaurantPOS.Services
{
    public interface INavigationService : INotifyPropertyChanged
    {
        object? CurrentView { get; }
        // Мэдэгдэж буй VM объект руу шууд navigate
        void NavigateTo(object viewModel);
        // DI-ээр VM үүсгээд (params аргументаар) navigate
        void NavigateTo<TViewModel>(params object[] args) where TViewModel : class;
    }

    public sealed class NavigationService : INavigationService
    {
        private readonly IServiceProvider _sp;

        public NavigationService(IServiceProvider serviceProvider)
            => _sp = serviceProvider;

        private object? _currentView;
        public object? CurrentView
        {
            get => _currentView;
            private set
            {
                if (!ReferenceEquals(_currentView, value))
                {
                    _currentView = value;
                    OnPropertyChanged();
                }
            }
        }

        public void NavigateTo(object viewModel)
        {
            if (viewModel is null) throw new ArgumentNullException(nameof(viewModel));
            CurrentView = viewModel;
        }

        public void NavigateTo<TViewModel>(params object[] args) where TViewModel : class
        {
            // DI + runtime аргументуудтайгаар VM үүсгэнэ
            var vm = ActivatorUtilities.CreateInstance<TViewModel>(_sp, args);
            CurrentView = vm;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
