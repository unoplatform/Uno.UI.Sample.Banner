using System;
using System.Reflection;

#if WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endif

namespace Uno.UI.Sample.Banner
{

	public enum AppRuntimeMode
	{
		Interpreted,
		Mixed,
		AOT
	}

	[ContentProperty(Name = "AppContent")]
	public partial class BannerControl : Control
	{
		private static AppRuntimeMode? _AppEnvironmentMode = null;

		private static string _applicationName;
		private static string _applicationCompany;
		private static string _applicationVersion;

		static BannerControl()
		{
			var runtimeMode = Environment.GetEnvironmentVariable("UNO_BOOTSTRAP_MONO_RUNTIME_MODE");

			if (!string.IsNullOrEmpty(runtimeMode))
			{ 
				if (runtimeMode.Equals("Interpreter", StringComparison.InvariantCultureIgnoreCase))
				{
					_AppEnvironmentMode = AppRuntimeMode.Interpreted;
				}
				else if (runtimeMode.Equals("FullAOT", StringComparison.InvariantCultureIgnoreCase))
				{
					_AppEnvironmentMode = AppRuntimeMode.AOT;
				}
				else if (runtimeMode.Equals("InterpreterAndAOT", StringComparison.InvariantCultureIgnoreCase))
				{
					_AppEnvironmentMode = AppRuntimeMode.Mixed;
				}
			}

			SetApplicationNameAndVersion();
		}

		private static void SetApplicationNameAndVersion()
		{
			var application = Application.Current;
			var assembly = application.GetType().GetTypeInfo().Assembly;
			var n = assembly.FullName;

#if DEBUG
            _applicationVersion = "1.0.0.dev.4+abcdabcdbacdbacdbacdabacadcadbacadcadbc";
#else
            if (assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>() is AssemblyInformationalVersionAttribute aiva)
			{
				_applicationVersion = aiva.InformationalVersion;
			}
			else if (assembly.GetCustomAttribute<AssemblyVersionAttribute>() is AssemblyVersionAttribute ava)
			{
				_applicationVersion = ava.Version;
			}
			else
			{
				_applicationVersion = "Unkown";
			}
#endif

			if(assembly.GetCustomAttribute<AssemblyProductAttribute>() is AssemblyProductAttribute apa)
			{
				_applicationName = apa.Product;
			}

			if(assembly.GetCustomAttribute<AssemblyCompanyAttribute>() is AssemblyCompanyAttribute aca)
			{
				_applicationCompany = aca.Company;
			}
		}


		public BannerControl()
		{
			DefaultStyleKey = typeof(BannerControl);
			AppName = _applicationName;
			AppAuthor = _applicationCompany;
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
					button.Tapped += (snd, evt) => _ = Windows.System.Launcher.LaunchUriAsync(new Uri(url));
				}
			}
		}

		private const string NvAssets = "https://uno-assets.platform.uno/app-banner-assets/";

		private string GetImageQuery()
		{
			return "?a=" + Uri.EscapeDataString(AppName) +
				"m=" + _AppEnvironmentMode +
				"&v=" + Uri.EscapeDataString(VersionNumber);
		}

		public string LogoGithub => NvAssets + "logo-github.png";
		public string LogoTwitter => NvAssets + "logo-twitter.png";
		public string LogoUno => NvAssets + "logo-uno.png" + GetImageQuery();
		public string LogoUnoSmall => NvAssets + "logo-uno-small.png" + GetImageQuery();

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

		public string AppEnvironmentMode => _AppEnvironmentMode.ToString();
		 
		public bool AppEnvironmentModeVisibility => _AppEnvironmentMode.HasValue;

		public bool InterpreterModeWarningVisibility => _AppEnvironmentMode == AppRuntimeMode.Interpreted;

		public string VersionNumber => _applicationVersion;

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
	}
}
