using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DASApprenticeManagementPOC.Views;

public partial class NavigationExample : ContentPage
{
    public NavigationExample()
    {
        InitializeComponent();
    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}