﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using StandardUtils.Helpers;
using StandardUtils.Models.Shared;

using Translation.Client.Web.Helpers;
using Translation.Client.Web.Helpers.ActionFilters;
using Translation.Client.Web.Helpers.DataResultHelpers;
using Translation.Client.Web.Helpers.Mappers;
using Translation.Client.Web.Models.Base;
using Translation.Client.Web.Models.TranslationProvider;
using Translation.Common.Contracts;
using Translation.Common.Models.Requests.Admin;
using Translation.Common.Models.Requests.TranslationProvider;

namespace Translation.Client.Web.Controllers
{
    public class TranslationProviderController : BaseController

    {
        private readonly ITranslationProviderService _translationProviderService;
        private readonly IAdminService _adminService;

        public TranslationProviderController(IOrganizationService organizationService,
                                             IJournalService journalService,
                                             ILanguageService languageService,
                                             ITranslationProviderService translationProviderService,
                                             IAdminService adminService) : base(organizationService, journalService, languageService, translationProviderService)
        {
            _translationProviderService = translationProviderService;
            _adminService = adminService;
        }

        [HttpGet]
        public ViewResult List()
        {
            var model = new TranslationProviderListModel();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListData(int skip, int take)
        {
            var request = new TranslationProviderReadListRequest();
            SetPaging(skip, take, request);

            var response = await _translationProviderService.GetTranslationProviders(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = DataResultHelper.GetTranslationProviderListDataResult(response.Items);
            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.PagingType = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeActivation(Guid id)
        {
            var adminUid = id;
            if (adminUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new TranslationProviderChangeActivationRequest(CurrentUser.Id, adminUid);
            var response = await _adminService.TranslationProviderChangeActivation(request);
            if (response.Status.IsNotSuccess)
            {
                return Json(new CommonResult { IsOk = false, Messages = response.ErrorMessages });
            }

            return Json(new CommonResult { IsOk = true });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var translationProviderUid = id;
            if (translationProviderUid.IsEmptyGuid())
            {
                return RedirectToAccessDenied();
            }

            var request = new TranslationProviderReadRequest(CurrentUser.Id, translationProviderUid);
            var translationProvider = await _translationProviderService.GetTranslationProvider(request);
            if (translationProvider.Status.IsNotSuccess)
            {
                return RedirectToAccessDenied();
            }

            var model = TranslationProviderMapper.MapTranslationProviderEditModel(translationProvider.Item);

            return View(model);
        }

        [HttpPost,
         JournalFilter(Message = "journal_translation_provider_edit")]
        public async Task<IActionResult> Edit(TranslationProviderEditModel model)
        {
            if (model.IsNotValid())
            {
                model.SetInputModelValues();
                return View(model);
            }

            var request = new TranslationProviderEditRequest(CurrentUser.Id, model.TranslationProviderUid, model.Value,
                                                             model.Description);
            var response = await _translationProviderService.EditTranslationProvider(request);
            if (response.Status.IsNotSuccess)
            {
                model.MapMessages(response);
                model.SetInputModelValues();
                return View(model);
            }

            CurrentUser.IsActionSucceed = true;
            return Redirect($"/TranslationProvider/Detail/{response.Item.Uid}");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var translationProviderUid = id;
            if (translationProviderUid.IsEmptyGuid())
            {
                return RedirectToAccessDenied();
            }

            var request = new TranslationProviderReadRequest(CurrentUser.Id, translationProviderUid);
            var response = await _translationProviderService.GetTranslationProvider(request);
            if (response.Status.IsNotSuccess)
            {
                return RedirectToAccessDenied();
            }

            var model = TranslationProviderMapper.MapTranslationProviderDetailModel(response.Item);
            return View(model);
        }
    }
}