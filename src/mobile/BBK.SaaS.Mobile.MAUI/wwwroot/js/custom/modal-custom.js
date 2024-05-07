
// Class Definition
var ModalCustomManagerService = function () {
    var show = function (id) {
        //$("#" + id).modal('show')
        var selector = "#" + id;
        const menuHider = document.querySelectorAll('.menu-hider');
        const menuSideBarLeft = document.querySelectorAll(selector);
        for (i = 0; i < menuSideBarLeft.length; i++) {
            menuSideBarLeft[i].classList.add("menu-active");
            menuSideBarLeft[i].style.display = "block";
            menuSideBarLeft[i].style.width = "95%";
            //menuSideBarLeft[i].style.height = "100%";
        }
        for (i = 0; i < menuHider.length; i++) {
            menuHider[i].classList.add("menu-active");

        }
    }

    var hide = function (id) {
        //$("#" + id).modal('hide')
        //Closing Menus
        const menuClose = document.querySelectorAll('.close-menu');
        menuClose.forEach(el => el.addEventListener('click', e => {
            const activeMenu = document.querySelectorAll('.menu-active');
            for (let i = 0; i < activeMenu.length; i++) { activeMenu[i].classList.remove('menu-active'); }
            for (let i = 0; i < wrappers.length; i++) { wrappers[i].style.transform = "translateX(-" + 0 + "px)" }
            var iframes = document.querySelectorAll('iframe');
            iframes.forEach(el => { var hrefer = el.getAttribute('src'); el.setAttribute('newSrc', hrefer); el.setAttribute('src', ''); var newSrc = el.getAttribute('newSrc'); el.setAttribute('src', newSrc) });
        }));

        const activeMenu = document.querySelectorAll('.menu-active');
        for (let i = 0; i < activeMenu.length; i++) { activeMenu[i].classList.remove('menu-active'); }
        var iframes = document.querySelectorAll('iframe');
        iframes.forEach(el => { var hrefer = el.getAttribute('src'); el.setAttribute('newSrc', hrefer); el.setAttribute('src', ''); var newSrc = el.getAttribute('newSrc'); el.setAttribute('src', newSrc) });
    }

    // Public Functions
    return {
        show: show,
        hide: hide
    };
}();