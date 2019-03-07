﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WeeklyFoodPlanner.Models;

namespace WeeklyFoodPlanner.Views
{
    public partial class NewMealPage : ContentPage
    {
        public Meal Meal { get; set; }

        public NewMealPage()
        {
            InitializeComponent();

            Meal = new Meal
            {
                Name = "Meal name",
                NumberDaysToRepeat = 1,
                Recipe = new Recipe {
                    Name = "Salmon Sashimi",
                    Description = "Delish"
                },
                MealTypes = HelperEnums.MealType.Breakfast
            };

            BindingContext = this;
        }

        async void Save_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddMeal", Meal);
            await Navigation.PopModalAsync();
        }

        async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}