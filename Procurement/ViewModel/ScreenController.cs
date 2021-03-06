using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using POEApi.Model;
using Procurement.View;
using Procurement.View.ViewModel;

namespace Procurement.ViewModel
{
    public class ScreenController : ObservableBase
    {
        private static Dictionary<string, IView> screens = new Dictionary<string, IView>();

        public double HeaderHeight { get; set; }
        public double FooterHeight { get; set; }

        public bool ButtonsVisible
        {
            get { return _buttonsVisible; }
            set { _buttonsVisible = value; OnPropertyChanged(); }
        }

        public bool FullMode { get; set; }

        public ICommand MenuButtonCommand => new RelayCommand(execute);

        private const string STASH_VIEW = "StashView";
        private const string RECIPE_VIEW = "Recipes";
        private const string TRADING_VIEW = "Trading";
        private const string INVENTORY_VIEW = "Inventory";
        private const string SETTINGS_VIEW = "Settings";
        private const string ABOUT_VIEW = "About";

        public static ScreenController Instance = null;
        private UserControl _selectedView;
        private bool _buttonsVisible;

        public static void Create()
        {
            Instance = new ScreenController();
        }

        private ScreenController()
        {
            FullMode = !bool.Parse(Settings.UserSettings["CompactMode"]) && !bool.Parse(Settings.UserSettings["MinimalMode"]);
            if (FullMode)
            {
                HeaderHeight = 169;
                FooterHeight = 138;
            }

            initLogin();
        }

        public void UpdateTrading()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    // TODO: As with updating the RecipeView, can we just update the trading view instead of recreating
                    // it?
                    screens[TRADING_VIEW] = new TradingView();
                }));
        }

        private void execute(object obj)
        {
            var key = obj.ToString();

            if (key == RECIPE_VIEW && screens[key] == null)
                screens[key] = new RecipeView();

            if (key == TRADING_VIEW && screens[key] == null)
                screens[key] = new TradingView();


            LoadView(screens[key]);
        }

        private void initScreens()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal,
            new Action(() =>
            {
                screens.Add(STASH_VIEW, new StashView());
                screens.Add(INVENTORY_VIEW, new InventoryView());
                screens.Add(TRADING_VIEW, null);
                screens.Add(SETTINGS_VIEW, new SettingsView());
                screens.Add(RECIPE_VIEW, null);
                screens.Add(ABOUT_VIEW, new AboutView());
            }));
        }

        public void InvalidateRecipeScreen()
        {
            screens[RECIPE_VIEW] = null;
        }

        public void RefreshRecipeScreen()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    // TODO: Cause the RecipeResultsViewModel in the RecipeView to refresh its recipes, instead of
                    // recreating the RecipeView object.  This could perhaps be done by triggering an event, or
                    // reaching into the view/viewmodel and calling it directly (but that's probably very bad form).
                    screens[RECIPE_VIEW] = new RecipeView();
                }));
        }

        private void initLogin()
        {
            var loginView = new LoginView();
            var loginVM = (loginView.DataContext as LoginWindowViewModel);
            loginVM.OnLoginCompleted += new LoginWindowViewModel.LoginCompleted(loginCompleted);
            LoadView(loginView);
        }

        void loginCompleted()
        {
            initScreens();
            LoadView(screens.First().Value);
            ButtonsVisible = true;
        }

        public UserControl SelectedView
        {
            get { return _selectedView; }
            set
            {
                _selectedView = value;
                OnPropertyChanged();
            }
        }

        public void LoadView(IView view)
        {
            SelectedView = view as UserControl;

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    SelectedView = view as UserControl;
                }));
        }

        public void LoadRefreshView()
        {
            ButtonsVisible = false;
            SelectedView = new RefreshView();
            (SelectedView as RefreshView).RefreshAllTabs();
        }

        public void LoadRefreshViewUsed()
        {
            ButtonsVisible = false;
            SelectedView = new RefreshView();
            (SelectedView as RefreshView).RefreshUsedTabs();
        }

        public void ReloadStash()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                new Action(() =>
                {
                    screens[STASH_VIEW] = new StashView();
                    SelectedView = screens[STASH_VIEW] as UserControl;
                    ButtonsVisible = true;
                }));
        }
    }
}