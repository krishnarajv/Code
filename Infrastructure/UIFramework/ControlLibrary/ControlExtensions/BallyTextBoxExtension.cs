﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Controls.ControlLibrary
{
    public static partial class ControlExtension
    {
        #region "Public Methods"

        public static MvcHtmlString BallyTextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isAutoComplete = false, StylePropertyBag style = null, short tabIndex = 0, string onLeaveFunction = "", string onKeyUpFunction = "", string onKeyDownFunction = "", string onChangeFunction = "", string cssClass = "", string autoCompleteInputFunction = "", IDictionary<string, object> attributes = null, bool? isEnabled = null, bool? isReadOnly = null)
        {
            string propertyName = string.Empty;
            string modelName = string.Empty;
            object value = string.Empty;
            string errMsg = string.Empty;
            string textBoxHTMLString = string.Empty;
            string configKey = string.Empty;
            Dictionary<string, string> overrideSettings;
            TextBoxHTMLEmitter textBoxHTMLEmitter;

            ControlExtension.GetPropertyNameAndValue<TModel, TProperty>(htmlHelper, expression, out propertyName, out modelName, out value, out errMsg, out configKey);

            overrideSettings = GetTextBoxOverrideSettings(style, tabIndex, onLeaveFunction, onKeyUpFunction, onKeyDownFunction, onChangeFunction, cssClass, autoCompleteInputFunction);

            var fillers = ControlPropertyFillerFactory.Get();
            FillerParams fillerParams = new FillerParams(modelName, propertyName, overrideSettings, attributes: attributes, isEnabled: isEnabled, isReadOnly: isReadOnly, configKey: configKey);
            var textBoxpropertyBag = new TextBoxPropertyBag(fillerParams);
            // THe below line of code needs to be placed before calling the Accept() method - dont change this
            textBoxpropertyBag.AutoComplete = isAutoComplete;
            textBoxpropertyBag.Accept(fillers);
            textBoxpropertyBag.ErrorMessage = errMsg;
            textBoxpropertyBag.IsDirty = string.IsNullOrEmpty(errMsg) ? false : true;

            value = GetTextBoxMaskingData<TModel>(value, textBoxpropertyBag);

            textBoxHTMLEmitter = new TextBoxHTMLEmitter(value != null ? value.ToString() : string.Empty, textBoxpropertyBag);
            textBoxHTMLEmitter.Emit(out textBoxHTMLString);
            return MvcHtmlString.Create(textBoxHTMLString);
        }

        #endregion "Public Methods"

        #region "Private Methods"

        private static object GetTextBoxMaskingData<TModel>(object value, TextBoxPropertyBag textBoxpropertyBag)
        {
            if (textBoxpropertyBag.Masking && textBoxpropertyBag.MaskingProperties != null)
            {
                string ciphetText = ControlLibraryConfig.EncryptionService.Encrypt(Convert.ToString(value));
                textBoxpropertyBag.EncryptedValue = ciphetText;
                string maskedData = GetMaskedString(Convert.ToString(value), textBoxpropertyBag.MaskingProperties);
                textBoxpropertyBag.ReadOnly = true;
                value = maskedData;
            }
            return value;
        }

        private static Dictionary<string, string> GetTextBoxOverrideSettings(StylePropertyBag style, short tabIndex, string onLeaveFunction, string onKeyUpFunction, string onKeyDownFunction, string onChangeFunction, string cssClass, string autoCompleteInputFunction)
        {
            Dictionary<string, string> overrideSettings;
            overrideSettings = new Dictionary<string, string>();
            overrideSettings.Add(ControlLibConstants.TAB_INDEX, tabIndex.ToString());
            overrideSettings.Add(ControlLibConstants.ON_LEAVE_FUNCTION, onLeaveFunction);
            overrideSettings.Add(ControlLibConstants.ON_KEY_UP_FUNCTION, onKeyUpFunction);
            overrideSettings.Add(ControlLibConstants.ON_KEY_DOWN_FUNCTION, onKeyDownFunction);
            overrideSettings.Add(ControlLibConstants.ON_CHANGE_FUNCTION, onChangeFunction);
            overrideSettings.Add(ControlLibConstants.CSS_CLASS, cssClass);
            overrideSettings.Add(ControlLibConstants.AUTOCOMPLETE_INPUT_FUNCTION, autoCompleteInputFunction);
            SetStyleSettings(style, overrideSettings);
            return overrideSettings;
        }

        #endregion "Private Methods"
    }
}