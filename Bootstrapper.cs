using Caliburn.Micro;
using WPF_Bestelbons.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPF_Bestelbons.Models;
using System.Windows.Input;
using WPF_Bestelbons.Gestures;

namespace WPF_Bestelbons
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;



        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<IDataServiceLeveranciers, DataServiceLeveranciers>();
            _container.RegisterSingleton(typeof(Leveranciers),null,typeof(Leveranciers));
            _container.RegisterSingleton(typeof(Bestelbon), null, typeof(Bestelbon));
            _container.RegisterSingleton(typeof(PDFCreatorBestelbon), null, typeof(PDFCreatorBestelbon));
            _container.RegisterSingleton(typeof(PDFCreatorPrijsaanvraag), null, typeof(PDFCreatorPrijsaanvraag));
            _container.RegisterSingleton(typeof(DSNSmtpClient), null, typeof(DSNSmtpClient));
            _container.Singleton<ShellViewModel>();
            _container.Singleton<InnerShellViewModel>();
            _container.Singleton<LeveranciersViewModel>();
            _container.Singleton<BestelbonsViewModel>();
            _container.Singleton<PrijsvragenViewModel>();
            _container.Singleton<BestelbonOpmaakViewModel>();
            _container.Singleton<PrijsvraagOpmaakViewModel>();
            _container.Singleton<AddLeverancierViewModel>();
            _container.Singleton<DialogViewModel>();
            _container.Singleton<SelectUserViewModel>();
            _container.Singleton<LeveringsvoorwaardenViewModel>();
            _container.Singleton<UsersViewModel>();
            _container.Singleton<EditLeverancierViewModel>();
            _container.Singleton<ProjectDirectoryViewModel>();
            _container.Singleton<ConfigurationViewModel>();
            _container.Singleton<ConfigurationDirectoriesViewModel>();
            _container.Singleton<ConfigurationProjectIDViewModel>();
            _container.Singleton<ProjectShellViewModel>();
            _container.Singleton<SendMailViewModel>();
            _container.Singleton<ProjectViewModel>();
            _container.Singleton<ConfigurationProjectsViewModel>();
            _container.Singleton<LoadViewModel>();
            _container.Singleton<ImportElDocBestelbonsViewModel>();


            var defaultCreateTrigger = Parser.CreateTrigger;

            //Parser.CreateTrigger = (target, triggerText) =>
            //{
            //    if (triggerText == null)
            //    {
            //        return defaultCreateTrigger(target, null);
            //    }

            //    var triggerDetail = triggerText
            //        .Replace("[", string.Empty)
            //        .Replace("]", string.Empty);

            //    var splits = triggerDetail.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            //    switch (splits[0])
            //    {
            //        case "Key":
            //            var key = (Key)Enum.Parse(typeof(Key), splits[1], true);
            //            return new KeyTrigger { Key = key };

            //        case "Gesture":
            //            var mkg = (MultiKeyGesture)(new MultiKeyGestureConverter()).ConvertFrom(splits[1]);
            //            return new KeyTrigger { Modifiers = mkg.KeySequences[0].Modifiers, Key = mkg.KeySequences[0].Keys[0] };
            //    }

            //    return defaultCreateTrigger(target, triggerText);
            //};


            MessageBinder.SpecialValues.Add("$pressedkey", (context) =>
            {
                // NOTE: IMPORTANT - you MUST add the dictionary key as lowercase as CM
                // does a ToLower on the param string you add in the action message, in fact ideally
                // all your param messages should be lowercase just in case. I don't really like this
                // behaviour but that's how it is!
                var keyArgs = context.EventArgs as KeyEventArgs;

                if (keyArgs != null)
                    return keyArgs.Key;

                return null;
            });



        }


        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;
            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

    }
}
