﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AasxCompatibilityModels;
using AasxIntegrationBase;
using AdminShellNS;
using Binding = System.Windows.Data.Binding;
using Label = System.Windows.Controls.Label;
using TextBox = System.Windows.Controls.TextBox;
using UserControl = System.Windows.Controls.UserControl;

namespace AasxPluginManifestViewer
{
    public partial class ManifestViewerViewControl : UserControl
    {
        private LogInstance Log = new LogInstance();

        public ManifestViewerViewControl()
        {
            InitializeComponent();
        }
        
        private AdminShellPackageEnv thePackage = null;
        private AdminShell.Submodel theSubmodel = null;
        private ManifestViewerOptions theOptions = null;
        private PluginEventStack theEventStack = null;

        private string theDefaultLang = null;

        public void Start(
            AdminShellPackageEnv package,
            AdminShell.Submodel sm,
            ManifestViewerOptions options,
            PluginEventStack eventStack)
        {
            // set the context
            this.thePackage = package;
            this.theSubmodel = sm;
            this.theOptions = options;
            this.theEventStack = eventStack;
            

            // ok, directly set contents
            SetContents();
        }

        private void AddLogo(AdminShell.SubmodelElementWrapper logo, Image logoImage)
        {
            var logoValue = (logo.submodelElement as AdminShell.File)?.value;

            thePackage.GetLocalStreamFromPackage(logoValue);
            using (var stream =
                   thePackage.GetLocalStreamFromPackage((logo.submodelElement as AdminShell.File)?.value))
            {
                stream.Position = 0;
                logoImage.Source = BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        private void TextChange(object sender, RoutedEventArgs e)
        {
            var context = (sender as TextBox).DataContext as AdminShell.Key;
            var newValue = (sender as TextBox).Text;
            var submodel = theSubmodel.submodelElements.FirstOrDefault(sm => sm.submodelElement.semanticId.Matches(context));
            
            //TODO: 

            var a = "a";
        }

        private void SetContents()
        {
            var rowDefinitions = theSubmodel.submodelElements.Select(
                sme => new RowDefinition()).ToList();
            rowDefinitions.ForEach(ManifestTable.RowDefinitions.Add);

            ManifestTable.ColumnDefinitions.Add(new ColumnDefinition());
            ManifestTable.ColumnDefinitions.Add(new ColumnDefinition());

            var fromLogo = theSubmodel.submodelElements.FirstOrDefault(sme =>
                sme.submodelElement.semanticId.Matches(Constants.FromLogoSemanticId));  
            
            var toLogo = theSubmodel.submodelElements.FirstOrDefault(sme =>
                sme.submodelElement.semanticId.Matches(Constants.ToLogoSemanticId));


            AddLogo(fromLogo, FromLogo);
            AddLogo(toLogo, ToLogo);


            theSubmodel.submodelElements.Select((sme, i) =>
            {
                if (!(sme.submodelElement is AdminShell.Property)) return 0;

                var label = new Label();
                var value = new TextBox();

                value.DataContext = sme.submodelElement.semanticId.First;
                value.LostFocus += TextChange;

                label.Content = sme.submodelElement.idShort;
                value.Text = sme.submodelElement.ValueAsText();

                Grid.SetRow(label, i);
                Grid.SetRow(value, i);

                Grid.SetColumn(label, 0);
                Grid.SetColumn(value, 1);

                ManifestTable.Children.Add(label);
                ManifestTable.Children.Add(value);

                return 0;
            }).ToList();


        }

        //TODO: Implement
        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender == ButtonPrint)
        //    {
        //        Print();
        //    }
        //}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Info("UserControl_Loaded");
            //// fill combo box
            //ComboBoxLanguage.ItemsSource = DefinitionsLanguages.DefaultLanguages;
            //ComboBoxLanguage.SelectedIndex = 0;
            //ComboBoxLanguage.SelectionChanged += (object sender2, SelectionChangedEventArgs e2) =>
            //{
            //    this.theDefaultLang = "" + ComboBoxLanguage.SelectedItem;
            //    SetContents();
            //    this.InvalidateVisual();
            //};
        }

        // see: https://stackoverflow.com/questions/13362448/how-to-draw-frameworkelement-on-drawingcontext
        public const double DPI = 96;

        /// <summary>
        /// This tells a visual to render itself to a wpf bitmap
        /// </summary>
        /// <remarks>
        /// This fixes an issue where the rendered image is blank:
        /// http://blogs.msdn.com/b/jaimer/archive/2009/07/03/rendertargetbitmap-tips.aspx
        /// </remarks>
        /// TODO: Logo
        //public static BitmapSource RenderControl(FrameworkElement visual, int width, int height, bool isInVisualTree)
        //{
        //    if (!isInVisualTree)
        //    {
        //        // If the visual isn't part of the visual tree, then it needs to be forced to finish its layout
        //        visual.Width = width;
        //        visual.Height = height;
        //        //  I thought these two statements would be expensive, but profiling shows it's mostly all on Render
        //        visual.Measure(new Size(width, height));
        //        visual.Arrange(new Rect(0, 0, width, height));
        //    }

        //    RenderTargetBitmap retVal = new RenderTargetBitmap(
        //        2 * width, 2 * height, 2 * DPI, 2 * DPI, PixelFormats.Pbgra32);

        //    DrawingVisual dv = new DrawingVisual();
        //    using (DrawingContext ctx = dv.RenderOpen())
        //    {
        //        VisualBrush vb = new VisualBrush(visual);
        //        ctx.DrawRectangle(vb, null, new Rect(new Point(0, 0), new Point(width, height)));
        //    }

        //    retVal.Render(dv);      //  profiling shows this is the biggest hit

        //    return retVal;
        //}

        // TODO
        //private void Print()
        //{
        //    // create header
        //    var cntlHeader = new TechDataHeaderControl();
        //    cntlHeader.SetContents(this.thePackage, this.theDefs, this.theDefaultLang, this.theSubmodel);

        //    // create footer
        //    var cntlFooter = new TechDataFooterControl();
        //    cntlFooter.SetContents(this.thePackage, this.theDefs, this.theDefaultLang, this.theSubmodel);

        //    // render this
        //    var bitmapHeader = RenderControl(cntlHeader, 600, 200, false);
        //    var bitmapFooter = RenderControl(cntlFooter, 600, 100, false);

        //    // create middle part
        //    var cntlProps = new TechDataPropertiesControl();
        //    cntlProps.SetContents(this.thePackage, this.theDefs, this.theDefaultLang, this.theSubmodel);
        //    var document = cntlProps.CreateFlowDocument(
        //        this.thePackage, this.theDefs, this.theDefaultLang, this.theSubmodel);

        //    // Clone the source document's content into a new FlowDocument.
        //    // This is because the pagination for the printer needs to be
        //    // done differently than the pagination for the displayed page.
        //    // We print the copy, rather that the original FlowDocument.
        //    System.IO.MemoryStream s = new System.IO.MemoryStream();
        //    TextRange source = new TextRange(document.ContentStart, document.ContentEnd);
        //    source.Save(s, DataFormats.Xaml);
        //    FlowDocument copy = new FlowDocument();
        //    TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
        //    dest.Load(s, DataFormats.Xaml);

        //    // Create a XpsDocumentWriter object, implicitly opening a Windows common print dialog,
        //    // and allowing the user to select a printer.

        //    // get information about the dimensions of the seleted printer+media.
        //    System.Printing.PrintDocumentImageableArea ia = null;
        //    System.Windows.Xps.XpsDocumentWriter docWriter =
        //        System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

        //    if (docWriter != null && ia != null)
        //    {
        //        // some more definitions
        //        var pdefs = new PimpedPaginator.Definition();
        //        pdefs.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
        //        pdefs.HeaderHeight = 200;
        //        pdefs.FooterHeight = 100;
        //        pdefs.RepeatTableHeaders = true;

        //        // draw header
        //        pdefs.Header = (DrawingContext context, Rect bounds, int pageNr) =>
        //        {
        //            context.DrawImage(bitmapHeader, bounds);
        //        };

        //        // draw footer
        //        pdefs.Footer = (DrawingContext context, Rect bounds, int pageNr) =>
        //        {
        //            context.DrawImage(bitmapFooter, bounds);
        //        };

        //        // make suitable paginator
        //        var paginator = new PimpedPaginator(copy, pdefs);

        //        // Change the PageSize and PagePadding for the document to match the CanvasSize for the printer device.
        //        paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
        //        Thickness t = new Thickness(72);
        //        copy.PagePadding = new Thickness(
        //                         Math.Max(ia.OriginWidth, t.Left),
        //                           Math.Max(ia.OriginHeight, t.Top),
        //                           Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
        //                           Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));

        //        copy.ColumnWidth = double.PositiveInfinity;

        //        // Send content to the printer.
        //        docWriter.Write(paginator);
        //    }
        //}
    }
}

