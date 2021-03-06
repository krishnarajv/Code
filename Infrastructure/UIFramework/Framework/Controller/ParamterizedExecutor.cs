﻿using Controls.ControlLibrary;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using Controls.Configuration;
using Controls.ExceptionHandling;
using Controls.Types;
using Controls.Security;

namespace Controls.Framework
{
    public sealed class ParamterizedExecutor<TCommand, TInput> : System.Web.Mvc.Controller
        where TCommand : ParameterizedActionCommand<TInput>
        where TInput : ViewModelBase
    {
        private TInput _viewModel = null;
        private ActionResult _actionResult = null;
        private readonly TCommand _command;
        private readonly IExecutionContext _executionCtx;
        private readonly ISessionContext _sessionCtx;

        public ParamterizedExecutor(IExecutionContext executionCtx, ISessionContext sessionCtx, TCommand command, IActionResultBuilder resultBuilder)
        {
            _command = command;
            _executionCtx = executionCtx;
            _sessionCtx = sessionCtx;
        }

        public ActionResult Process(TInput viewModel)
        {
            try
            {
                this._viewModel = viewModel;

                //Setting the configuration key in model from the command
                this._viewModel.ConfigurationKey = _command.GetConfigurationKey();
                
                if (this._viewModel.Validate())
                {
                    _executionCtx.SafeActionBlock.Invoke(this.ExecuteProcess, null, null);
                }
                else
                {
                    _executionCtx.ResponseHeader.Add(Constants.ISVALID, "False");
                    _actionResult = Json(this._viewModel.ErrorList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (ViewException viewException)
            {
                ControllerConfigurator.utilityProvider.GetLogger().LogFatal("ParamterizedExecutor.ExecuteProcess.ViewException", viewException);
                _actionResult = viewException.BuildException(viewException);
                _executionCtx.ResponseHeader.Add(Constants.EXCEPTIONHEADER, "VIEW");
            }
            catch (JsonException jsonException)
            {
                ControllerConfigurator.utilityProvider.GetLogger().LogFatal("ParamterizedExecutor.ExecuteProcess.JsonException", jsonException);
                _actionResult = jsonException.BuildException(jsonException);
                _executionCtx.ResponseHeader.Add(Constants.EXCEPTIONHEADER, "JSON");
            }
            finally
            {
                BuildResponseHeader();
            }
            return _actionResult;
        }
        private void BuildResponseHeader()
        {
            foreach (KeyValuePair<string, string> header in _executionCtx.ResponseHeader)
            {
                Response.AppendHeader(header.Key, header.Value);
            }
        }

        private void ExecuteProcess()
        {
            try
            {
                _command.Do(_executionCtx, _sessionCtx, _viewModel);
            }
            catch (ModuleException moduleException)
            {
                throw _command.ExceptionHandler(moduleException);
            }
            catch (Exception exception)
            {
                throw _command.UIExceptionHandler(exception);
            }
        }
    }
}