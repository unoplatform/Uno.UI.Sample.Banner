using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Uno.UI.Sample.Banner
{
	[ContentProperty(Name = "AppContent")]
	public partial class BannerControl : Control
	{
		public BannerControl()
		{
			DefaultStyleKey = typeof(BannerControl);
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			RegisterButtonForLink("UnoButton", "https://platform.uno");
			RegisterButtonForLink("appButton", LinkToUnoPlatformApp);
			RegisterButtonForLink("authorButton", LinkToOriginalApp);
			RegisterButtonForLink("TwitterButton", "https://twitter.com/UnoPlatform");
			RegisterButtonForLink("GithubButton", LinkToUnoPlatformApp);
			RegisterButtonForLink("visitUnoWebsiteButton", "https://platform.uno");

			// LinkToAppAuthor is not used!

			if(GetTemplateChild("closeAboutButton") is Button closeAboutButton
				&& GetTemplateChild("aboutPopup") is Flyout aboutPopup)
			{
				closeAboutButton.Tapped += (snd, evt) => aboutPopup.Hide();
			}
		}

		private void RegisterButtonForLink(string templateName, string url)
		{
			if (GetTemplateChild(templateName) is Button button)
			{
				if (string.IsNullOrEmpty(url))
				{
					button.Visibility = Visibility.Collapsed;
				}
				else
				{
					button.Tapped += (snd, evt) => Windows.System.Launcher.LaunchUriAsync(new Uri(url));
				}
			}
		}

		private const string NvAssets = "https://nv-assets.azurewebsites.net/app-banner-assets/";

		public string LogoGithub { get; set; } = NvAssets + "logo-github.png";
		public string LogoTwitter { get; set; } = NvAssets + "logo-twitter.png";
		public string LogoUno { get; set; } = NvAssets + "logo-uno.png";
		public string LogoUnoSmall { get; set; } = NvAssets + "logo-uno-small.png";

		public static readonly DependencyProperty AppNameProperty =
			DependencyProperty.Register("AppName", typeof(string), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty AppAuthorProperty =
			DependencyProperty.Register("AppAuthor", typeof(string), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty LinkToOriginalAppProperty =
			DependencyProperty.Register("LinkToOriginalApp", typeof(string), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty LinkToAppAuthorProperty =
			DependencyProperty.Register("LinkToAppAuthor", typeof(string), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty LinkToUnoPlatformAppProperty =
			DependencyProperty.Register("LinkToUnoPlatformApp", typeof(string), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty AppEnvironmentModeProperty =
			DependencyProperty.Register("AppEnvironmentMode", typeof(string), typeof(BannerControl), new PropertyMetadata(null, OnAppEnvironmentModeChanged));
		public static readonly DependencyProperty AppEnvironmentModeVisibilityProperty =
			DependencyProperty.Register("AppEnvironmentModeVisibility", typeof(Visibility), typeof(BannerControl), new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty InterpreterModeWarningVisibilityProperty =
			DependencyProperty.Register("InterpreterModeWarningVisibility", typeof(Visibility), typeof(BannerControl), new PropertyMetadata(Visibility.Collapsed));
		public static readonly DependencyProperty VersionNumberProperty =
			DependencyProperty.Register("VersionNumber", typeof(string), typeof(BannerControl), new PropertyMetadata(GetApplicationVersion()));
		public static readonly DependencyProperty AboutContentProperty =
			DependencyProperty.Register("AboutContent", typeof(object), typeof(BannerControl), new PropertyMetadata(null));
		public static readonly DependencyProperty AppContentProperty =
			DependencyProperty.Register("AppContent", typeof(object), typeof(BannerControl), new PropertyMetadata(null));

		public string AppName
		{
			get => (string)GetValue(AppNameProperty);
			set => SetValue(AppNameProperty, value);
		}

		public string AppAuthor
		{
			get => (string)GetValue(AppAuthorProperty);
			set => SetValue(AppAuthorProperty, value);
		}

		public string LinkToOriginalApp
		{
			get => (string)GetValue(LinkToOriginalAppProperty);
			set => SetValue(LinkToOriginalAppProperty, value);
		}

		public string LinkToAppAuthor
		{
			get => (string)GetValue(LinkToAppAuthorProperty);
			set => SetValue(LinkToAppAuthorProperty, value);
		}

		public string LinkToUnoPlatformApp
		{
			get => (string)GetValue(LinkToUnoPlatformAppProperty);
			set => SetValue(LinkToUnoPlatformAppProperty, value);
		}

		public string AppEnvironmentMode
		{
			get => (string)GetValue(AppEnvironmentModeProperty);
			set => SetValue(AppEnvironmentModeProperty, value);
		}

		public Visibility AppEnvironmentModeVisibility
		{
			get => (Visibility)GetValue(AppEnvironmentModeVisibilityProperty);
			set => SetValue(AppEnvironmentModeVisibilityProperty, value);
		}

		public Visibility InterpreterModeWarningVisibility
		{
			get => (Visibility)GetValue(InterpreterModeWarningVisibilityProperty);
			set => SetValue(InterpreterModeWarningVisibilityProperty, value);
		}

		public string VersionNumber
		{
			get => (string)GetValue(VersionNumberProperty);
			set => SetValue(VersionNumberProperty, value);
		}

		public object AboutContent
		{
			get => (object)GetValue(AboutContentProperty);
			set => SetValue(AboutContentProperty, value);
		}

		public object AppContent
		{
			get => (object)GetValue(AppContentProperty);
			set => SetValue(AppContentProperty, value);
		}

		private static void OnAppEnvironmentModeChanged(object d, DependencyPropertyChangedEventArgs e)
		{
			if (d is BannerControl banner)
			{
				var newValue = e.NewValue as string;
				var isInterpreted = "Interpreted".Equals(newValue, StringComparison.OrdinalIgnoreCase);

				banner.AppEnvironmentModeVisibility = string.IsNullOrEmpty(newValue) ? Visibility.Collapsed : Visibility.Visible;
				banner.InterpreterModeWarningVisibility = isInterpreted ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		private static string GetApplicationVersion()
		{
			var application = Application.Current;
			var assembly = application.GetType().GetTypeInfo().Assembly;

			if (assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>() is AssemblyInformationalVersionAttribute aiva)
			{
				return aiva.InformationalVersion;
			}

			if (assembly.GetCustomAttribute<AssemblyVersionAttribute>() is AssemblyVersionAttribute ava)
			{
				return ava.Version;
			}

			return "Unkown";
		}
	}
}
