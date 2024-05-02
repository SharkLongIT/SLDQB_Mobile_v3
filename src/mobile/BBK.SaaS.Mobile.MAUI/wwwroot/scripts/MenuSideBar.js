function menuSideBar() {
    ////Opening Menus
    //const menuOpen = document.querySelectorAll('.opening-menu');
    //const menuHider = document.querySelectorAll('.menu-hider');
    //menuOpen.forEach(el => el.addEventListener('click', e => {
    //    const menuSideBarLeft = document.querySelectorAll('#menu-sidebar-left');
    //    for (i = 0; i < menuSideBarLeft.length; i++) {
    //        menuSideBarLeft[i].classList.add("menu-active");
    //        menuSideBarLeft[i].style.display = "block";
    //        menuSideBarLeft[i].style.width = "320px";
    //    }
    //    for (i = 0; i < menuHider.length; i++) {
    //        menuHider[i].classList.add("menu-active");

    //    }
    //    //menuOpen.classList.add('menu-active');
    //    //menuHider.classList.add('menu-active')

    //}));


    ////Closing Menus
    //const menuClose = document.querySelectorAll('.close-menu, .menu-hider');
    //menuClose.forEach(el => el.addEventListener('click', e => {
    //    const activeMenu = document.querySelectorAll('.menu-active');
    //    for (let i = 0; i < activeMenu.length; i++) { activeMenu[i].classList.remove('menu-active'); }
    //    var iframes = document.querySelectorAll('iframe');
    //    iframes.forEach(el => { var hrefer = el.getAttribute('src'); el.setAttribute('newSrc', hrefer); el.setAttribute('src', ''); var newSrc = el.getAttribute('newSrc'); el.setAttribute('src', newSrc) });
    //}));
    //alert("JavaScript");
    const menuHider = document.querySelectorAll('.menu-hider');
    const menuSideBarLeft = document.querySelectorAll('#menu-sidebar-left');
        for (i = 0; i < menuSideBarLeft.length; i++) {
            menuSideBarLeft[i].classList.add("menu-active");
            menuSideBarLeft[i].style.display = "block";
            menuSideBarLeft[i].style.width = "320px";
        }
        for (i = 0; i < menuHider.length; i++) {
            menuHider[i].classList.add("menu-active");

    }
    //Closing Menus
    const menuClose = document.querySelectorAll('.close-menu, .menu-hider');
    menuClose.forEach(el => el.addEventListener('click', e => {
        const activeMenu = document.querySelectorAll('.menu-active');
        for (let i = 0; i < activeMenu.length; i++) { activeMenu[i].classList.remove('menu-active'); }
        var iframes = document.querySelectorAll('iframe');
        iframes.forEach(el => { var hrefer = el.getAttribute('src'); el.setAttribute('newSrc', hrefer); el.setAttribute('src', ''); var newSrc = el.getAttribute('newSrc'); el.setAttribute('src', newSrc) });
    }));

    //Click Menus
    const Link = document.querySelectorAll('.menu-box-left a');
    Link.forEach(el => el.addEventListener('click', e => {
        const activeMenu = document.querySelectorAll('.menu-active');
        for (let i = 0; i < activeMenu.length; i++) { activeMenu[i].classList.remove('menu-active'); }
        var iframes = document.querySelectorAll('iframe');
        iframes.forEach(el => { var hrefer = el.getAttribute('src'); el.setAttribute('newSrc', hrefer); el.setAttribute('src', ''); var newSrc = el.getAttribute('newSrc'); el.setAttribute('src', newSrc) });
    }));
}

