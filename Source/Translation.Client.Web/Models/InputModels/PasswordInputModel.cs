﻿namespace Translation.Client.Web.Models.InputModels
{
    public class PasswordInputModel : InputModel
    {
        public PasswordInputModel(string name, string labelKey, bool isRequired = false, string value = "") : base(name, labelKey, isRequired, value)
        {
        }
    }
}