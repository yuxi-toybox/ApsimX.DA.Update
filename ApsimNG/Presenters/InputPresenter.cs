﻿// -----------------------------------------------------------------------
// <copyright file="InputPresenter.cs" company="APSIM Initiative">
//     Copyright (c) APSIM Initiative
// </copyright>
// -----------------------------------------------------------------------

namespace UserInterface.Presenters
{
    using System;
    using global::UserInterface.Interfaces;
    using Models;
    using Models.Core;
    using Utility;
    using Views;

    /// <summary>
    /// Attaches an Input model to an Input View.
    /// </summary>
    public class InputPresenter : IPresenter
    {
        /// <summary>
        /// The input model
        /// </summary>
        private Models.PostSimulationTools.Input input;

        /// <summary>
        /// The input view
        /// </summary>
        private IInputView view;

        /// <summary>
        /// The Explorer
        /// </summary>
        private ExplorerPresenter explorerPresenter;

        /// <summary>
        /// Attaches an Input model to an Input View.
        /// </summary>
        /// <param name="model">The model to attach</param>
        /// <param name="view">The View to attach</param>
        /// <param name="explorerPresenter">The explorer</param>
        public void Attach(object model, object view, ExplorerPresenter explorerPresenter)
        {
            this.input = model as Models.PostSimulationTools.Input;
            this.view = view as IInputView;
            this.explorerPresenter = explorerPresenter;
            this.view.BrowseButtonClicked += this.OnBrowseButtonClicked;

            this.OnModelChanged(model);  // Updates the view

            this.explorerPresenter.CommandHistory.ModelChanged += this.OnModelChanged;
        }

        /// <summary>
        /// Detaches an Input model from an Input View.
        /// </summary>
        public void Detach()
        {
            this.view.BrowseButtonClicked -= this.OnBrowseButtonClicked;
            this.explorerPresenter.CommandHistory.ModelChanged -= this.OnModelChanged;
        }

        /// <summary>
        /// Browse button was clicked by user.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">The params</param>
        private void OnBrowseButtonClicked(object sender, EventArgs e)
        {
            try
            {
                IFileDialog dialog = new FileDialog()
                {
                    Prompt = "Choose files",
                    Action = FileDialog.FileActionType.Open,
                    FileType = "CSV Files (*.csv) | *.csv | All Files (*.*) | *.*",
                };
                string[] files = dialog.GetFiles();
                if (files != null && files.Length > 0)
                    explorerPresenter.CommandHistory.Add(new Commands.ChangeProperty(input, "FullFileNames", files));
            }
            catch (Exception err)
            {
                explorerPresenter.MainPresenter.ShowError(err);
            }
        }

        /// <summary>
        /// The model has changed - update the view.
        /// </summary>
        /// <param name="changedModel">The model object</param>
        private void OnModelChanged(object changedModel)
        {
            if (input.FullFileNames != null)
                view.FileName = string.Join(", ", input.FullFileNames);

            if (input.FullFileNames != null && input.FullFileNames.Length > 0)
            this.view.GridView.DataSource = this.input.GetTable(input.FullFileNames[0]);
        }
    }
}
