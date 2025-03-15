using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Greenhouse
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Greenhouse greenhouse;

		public MainWindow()
		{
			InitializeComponent();

			TimeSpan showerInterval = TimeSpan.FromSeconds(3);

			ComboBox[] comboBoxes = { ComboOne, ComboTwo, ComboThree, ComboFour };
			Image[] images = { PotOneSprinkler, PotTwoSprinkler, PotThreeSprinkler, PotFourSprinkler };

			Plant[] plants = { new Plant("Горохостріл", peashooter, Log, ComboOne), new Plant("Горіх", wallnut, Log, ComboTwo),
				new Plant("Кабачок", cabage, Log, ComboThree), new Plant("Соняшник", sunflower, Log, ComboFour) };

			ShowerPots showerPots = new ShowerPots(comboBoxes, images, showerInterval, Log);

			greenhouse = new Greenhouse(comboBoxes, images, showerInterval, Log, plants, showerPots);
		}

		public class ShowerPots
		{
			ComboBox[] comboBoxes;
			Image[] images;
			TimeSpan interval;
			TextBlock log;
			public ShowerPots(ComboBox[] comboBoxes, Image[] images, TimeSpan interval, TextBlock log)
			{
				this.comboBoxes = comboBoxes;
				this.images = images;
				this.interval = interval;
				this.log = log;
			}
			public async Task ShowerLow()
			{
				for (int i = 0; i < comboBoxes.Length; i++)
				{
					if (comboBoxes[i].Text == "Слабкий полив")
					{
						images[i].Opacity = 100;
						log.Text = "";
						log.Text += $" {i + 1}-ий вазон поливається (Слабкий полив)";
						await Task.Delay(interval);
						images[i].Opacity = 0;
					}
				}
			}
			public async Task ShowerMedium()
			{
				for (int i = 0; i < comboBoxes.Length; i++)
				{
					if (comboBoxes[i].Text == "Середній полив")
					{
						images[i].Opacity = 100;
						log.Text = "";
						log.Text += $" {i + 1}-ий вазон поливається (Середній полив)";
						await Task.Delay(interval);
						images[i].Opacity = 0;
					}
				}
			}
			public async Task ShowerStrong()
			{
				for (int i = 0; i < comboBoxes.Length; i++)
				{
					if (comboBoxes[i].Text == "Сильний полив")
					{
						images[i].Opacity = 100;
						log.Text = "";
						log.Text += $" {i + 1}-ий вазон поливається (Сильний полив)";
						await Task.Delay(interval);
						images[i].Opacity = 0;
					}
				}
			}
		}

		public class Plant
		{
			int growthPercentage = 0;
			string name;
			Image image;
			TextBlock log;
			ComboBox comboBox;

			bool grownToFifty = false;
			bool grownToHundread = false;
			public Plant(string name, Image image, TextBlock log, ComboBox comboBox)
			{
				this.name = name;
				this.image = image;
				this.log = log;
				this.comboBox = comboBox;
			}
			public async Task Grow()
			{
				if (grownToHundread == false)
				{
					if (comboBox.Text == "Слабкий полив")
					{
						growthPercentage += 3;
					}
					else if (comboBox.Text == "Середній полив")
					{
						growthPercentage += 6;
					}
					else if (comboBox.Text == "Сильний полив")
					{
						growthPercentage += 50;
					}

					log.Text = "";
					log.Text = $" Процент зросту {name}: {growthPercentage}";
					await Task.Delay(TimeSpan.FromSeconds(2));
				}
				if (growthPercentage >= 50 && grownToFifty == false)
				{
					image.Height += 20;
					image.Width += 20;
					image.Margin = new Thickness(image.Margin.Left - 10, image.Margin.Top - 10, 0, 0);
					log.Text = "";
					log.Text = $" {name} виріс до 50%";
					await Task.Delay(TimeSpan.FromSeconds(2));
					grownToFifty = true;
				}
				else if (growthPercentage >= 100 && grownToHundread == false)
				{
					image.Height += 20;
					image.Width += 20;
					image.Margin = new Thickness(image.Margin.Left - 10, image.Margin.Top - 10, 0, 0);
					log.Text = "";
					log.Text = $" {name} виріс до 100%";
					await Task.Delay(TimeSpan.FromSeconds(2));
					grownToHundread = true;
				}
			}
		}

		public class Greenhouse
		{
			ComboBox[] comboBoxes;
			Image[] images;
			TimeSpan interval;
			TextBlock log;
			ShowerPots showerPots;
			Plant[] plants;

			public Greenhouse(ComboBox[] comboBoxes, Image[] images, TimeSpan interval, TextBlock log, Plant[] plants, ShowerPots showerPots)
			{
				this.comboBoxes = comboBoxes;
				this.images = images;
				this.interval = interval;
				this.log = log;
				this.plants = plants;
				this.showerPots = showerPots;
			}

			public async Task ShowerPlants()
			{
				await showerPots.ShowerLow();
				for (int i = 0; i < plants.Length; i++)
				{
					if (comboBoxes[i].Text == "Слабкий полив")
					{
						await plants[i].Grow();
					}
				}

				await showerPots.ShowerMedium();
				for (int i = 0; i < plants.Length; i++)
				{
					if (comboBoxes[i].Text == "Середній полив")
					{
						await plants[i].Grow();
					}
				}

				await showerPots.ShowerStrong();
				for (int i = 0; i < plants.Length; i++)
				{
					if (comboBoxes[i].Text == "Сильний полив")
					{
						await plants[i].Grow();
					}
				}
			}
		}


		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			HideButtons();

			await greenhouse.ShowerPlants();

			ShowButtons();
		}

		public void HideButtons()
		{
			ShowerBut.Visibility = Visibility.Hidden;

			ComboOne.Visibility = Visibility.Hidden;
			ComboTwo.Visibility = Visibility.Hidden;
			ComboThree.Visibility = Visibility.Hidden;
			ComboFour.Visibility = Visibility.Hidden;
		}

		public void ShowButtons()
		{
			ShowerBut.Visibility = Visibility.Visible;

			ComboOne.Visibility = Visibility.Visible;
			ComboTwo.Visibility = Visibility.Visible;
			ComboThree.Visibility = Visibility.Visible;
			ComboFour.Visibility = Visibility.Visible;
		}
	}
}
