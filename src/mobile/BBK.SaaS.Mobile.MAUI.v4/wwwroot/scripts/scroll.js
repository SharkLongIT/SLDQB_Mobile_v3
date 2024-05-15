//Scroll Ads
function scroll() {
    var scrollItems = document.querySelectorAll('.scroll-ad, .header-auto-show')
    if (scrollItems.length) {
        var scrollAd = document.querySelectorAll('.scroll-ad');
        var scrollHeader = document.querySelectorAll('.header-auto-show');
        window.addEventListener('scroll', function () {
            if (document.querySelectorAll('.scroll-ad, .header-auto-show').length) {
                function showScrollAd() { scrollAd[0].classList.add('scroll-ad-visible'); }
                function hideScrollAd() { scrollAd[0].classList.remove('scroll-ad-visible'); }
                function showHeader() { scrollHeader[0].classList.add('header-active'); }
                function hideHeader() { scrollHeader[0].classList.remove('header-active'); }
                var window_height = window.outerWidth;
                var total_scroll_height = document.documentElement.scrollTop
                let inside_header = total_scroll_height <= 150;
                var passed_header = total_scroll_height >= 150;
                let inside_footer = (window_height - total_scroll_height + 1000) <= 150
                if (scrollAd.length) {
                    inside_header ? hideScrollAd() : null
                    passed_header ? showScrollAd() : null
                    inside_footer ? hideScrollAd() : null
                }
                if (scrollHeader.length) {
                    inside_header ? hideHeader() : null
                    passed_header ? showHeader() : null
                }
            }
        });
    }
}