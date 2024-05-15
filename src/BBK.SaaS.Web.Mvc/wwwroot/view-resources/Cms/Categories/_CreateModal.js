(function () {
  app.modals.CreateCmsCatModal = function () {
	var _modalManager;
	var _categoryService = abp.services.app.cmsCats;
	var _$form = null;

	this.init = function (modalManager) {
	  _modalManager = modalManager;

	  _$form = _modalManager.getModal().find('form[name=CategoryForm]');
	  _$form.validate({ ignore: '' });

	  var tagWhitelist = [];
	  _categoryService.getCmsCats({}).done(function (result) {

		tagWhitelist = _.map(result.items, function (item) {
		  return {
			id: item.id,
			value: item.displayName,
			name: item.displayName
		  };
		});

		//$.each(result.items, function (i) {
		//  console.log(this, i);
		//  tagWhitelist.push(this.displayName);
		//});
		//console.log(tagWhitelist);

		var input = document.querySelector("input[name=CategoryRelated]"),
		  // init Tagify script on the above inputs
		  tagify = new Tagify(input, {
			whitelist: tagWhitelist,
			dropdown: {
			  position: "manual",
			  maxItems: Infinity,
			  enabled: 0,
			  classname: "customSuggestionsList",
			},
			templates: {
			  dropdownItemNoMatch() {
				return `<div class='empty'>Nothing Found</div>`;
			  },
			},
			enforceWhitelist: true,
		  });

		tagify
		  .on("dropdown:show", onSuggestionsListUpdate)
		  .on("dropdown:hide", onSuggestionsListHide)
		  .on("dropdown:scroll", onDropdownScroll);

		renderSuggestionsList(); // defined down below

		// ES2015 argument destructuring
		function onSuggestionsListUpdate({ detail: suggestionsElm }) {
		  console.log(suggestionsElm);
		}

		function onSuggestionsListHide() {
		  console.log("hide dropdown");
		}

		function onDropdownScroll(e) {
		  console.log(e.detail);
		}

		// https://developer.mozilla.org/en-US/docs/Web/API/Element/insertAdjacentElement
		function renderSuggestionsList() {
		  tagify.dropdown.show(); // load the list
		  tagify.DOM.scope.parentNode.appendChild(tagify.DOM.dropdown);
		}


	  });

	  //console.log(_modalManager.getModal().getOptions());

	  //var cats = $('input[name=Categories]').val();
	  //console.log(cats);

	  //$.each(cats, function (i) { console.log(this, i); tagWhitelist.push(i.DisplayName); });





	};

	this.save = function () {
	  if (!_$form.valid()) {
		return;
	  }

	  _$formSEO = _modalManager.getModal().find('form[name=CategorySEOForm]');
	  var item = $.extend(_$form.serializeFormToObject(), _$formSEO.serializeFormToObject());

	  console.log(item);

	  _modalManager.setBusy(true);
	  _categoryService
		.createCmsCat(item)
		.done(function (result) {
		  abp.notify.info(app.localize('SavedSuccessfully'));
		  _modalManager.setResult(result);
		  _modalManager.close();
		})
		.always(function () {
		  _modalManager.setBusy(false);
		});
	};
  };
})();
