﻿using System;

using Translation.Client.Web.Models.Label;
using Translation.Client.Web.Models.LabelTranslation;
using Translation.Client.Web.Models.TranslationProvider;
using Translation.Common.Models.DataTransferObjects;


namespace Translation.Client.Web.Helpers.Mappers
{
    public class TranslationProviderMapper
    {
        public static TranslationProviderEditModel MapTranslationProviderEditModel(
                   TranslationProviderDto translationProvider)
        {
            var model = new TranslationProviderEditModel();

            model.TranslationProviderUid = translationProvider.Uid;
            model.Value = translationProvider.Value;
            model.Name = translationProvider.Name;
            model.Description = translationProvider.Description;

            model.SetInputModelValues();
            return model;
        }

    }
}