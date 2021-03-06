﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using WeeklyFoodPlanner.Models;
using WeeklyFoodPlanner.Services;
using WeeklyFoodPlanner.Views;
using Xamarin.Forms;

namespace WeeklyFoodPlanner.ViewModels
{

    public class PlannerViewModel : BaseViewModel
    {
        public DayOfWeek Today { get => DateTime.Today.DayOfWeek; }

        // use this to keep a full unfiltered list of all meals
        public List<Meal> AllItems { get; set; }

        private IMealsRepository _repo = new MealsRepository();

        // change all items to meal
        // change all recipe level to meal
        private ObservableCollection<Meal> items;

        public ObservableCollection<Meal> Items { get => items; set => SetProperty(ref items, value); }
        public Command LoadItemsCommand { get; set; }
        public Command WeekDayCommand { get; set; }

        public PlannerViewModel()
        {
            Title = "Weekly Planner";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));

            Items = new ObservableCollection<Meal>();
            //Items = new ObservableCollection<Recipe>(_repo.GetMealsAsync().Result);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            WeekDayCommand = new Command(day => ExecuteWeekDayCommand(day));

            MessagingCenter.Subscribe<NewMealPage, Meal>(this, "AddMeal", async (obj, item) =>
            {
                var newItem = item as Meal;
                Items.Add(newItem);
                await MealDataStore.AddAsync(newItem);
            });
        }

        private void ExecuteWeekDayCommand(object selectedDay)
        {
            Enum.TryParse(selectedDay.ToString(), out DayOfWeek day);

            if (AllItems != null)
            {
                var result = AllItems.Where(meal => meal.Days.Contains(day))
                    .OrderBy(meal => (int)(meal.MealType));

                Items.Clear();

                foreach (var item in result) Items.Add(item);
            }
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await MealDataStore.GetAllAsync(true);

                AllItems = items.ToList();

                ExecuteWeekDayCommand(DateTime.Today.DayOfWeek);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public ICommand OpenWebCommand { get; }
    }
}