

//function removeTransitions() { var falseTransitions = document.querySelectorAll('.btn, .header, #footer-bar, .menu-box, .menu-active'); for (let i = 0; i < falseTransitions.length; i++) { falseTransitions[i].style.transition = "all 0s ease"; } }
//function addTransitions() { var trueTransitions = document.querySelectorAll('.btn, .header, #footer-bar, .menu-box, .menu-active'); for (let i = 0; i < trueTransitions.length; i++) { trueTransitions[i].style.transition = ""; } }
//Page Highlights
function footerBar() {
    alert("hi");
var highlightData = document.querySelectorAll('[data-change-highlight]');
highlightData.forEach(el => el.addEventListener('click', e => {
    var highlight = el.getAttribute('data-change-highlight');
    var pageHighlight = document.querySelectorAll('.page-highlight');
    if (pageHighlight.length) { pageHighlight.forEach(function (e) { e.remove(); }); }
    var loadHighlight = document.createElement("link");
    loadHighlight.rel = "stylesheet";
    loadHighlight.className = "page-highlight";
    loadHighlight.type = "text/css";
    loadHighlight.href = 'styles/highlights/highlight_' + highlight + '.css';
    document.getElementsByTagName("head")[0].appendChild(loadHighlight);
    document.body.setAttribute('data-highlight', 'highlight-' + highlight)
    localStorage.setItem(pwaName + '-Highlight', highlight)
}))
var rememberHighlight = localStorage.getItem(pwaName + '-Highlight');
if (rememberHighlight) {
    document.body.setAttribute('data-highlight', rememberHighlight);
    var loadHighlight = document.createElement("link");
    loadHighlight.rel = "stylesheet";
    loadHighlight.className = "page-highlight";
    loadHighlight.type = "text/css";
    loadHighlight.href = 'styles/highlights/highlight_' + rememberHighlight + '.css';
    if (!document.querySelectorAll('.page-highlight').length) {
        document.getElementsByTagName("head")[0].appendChild(loadHighlight);
        document.body.setAttribute('data-highlight', 'highlight-' + rememberHighlight)
    }
} else {
    var bodyHighlight = document.body.getAttribute('data-highlight');
    var defaultHighlight = bodyHighlight.split("highlight-")
    document.body.setAttribute('data-highlight', defaultHighlight[1]);
    var loadHighlight = document.createElement("link");
    loadHighlight.rel = "stylesheet";
    loadHighlight.className = "page-highlight";
    loadHighlight.type = "text/css";
    loadHighlight.href = 'styles/highlights/highlight_' + defaultHighlight[1] + '.css';
    if (!document.querySelectorAll('.page-highlight').length) {
        document.getElementsByTagName("head")[0].appendChild(loadHighlight);
        document.body.setAttribute('data-highlight', 'highlight-' + defaultHighlight[1])
        localStorage.setItem(pwaName + '-Highlight', defaultHighlight[1])
    }
}
}

