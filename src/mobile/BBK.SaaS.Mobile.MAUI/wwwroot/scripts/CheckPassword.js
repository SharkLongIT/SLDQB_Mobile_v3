function checkPassword() {
    $(document).ready(function () {
        $('.password-field input').keyup(function () {
            var strength = checkPasswordStrength($(this).val());
            var $outputTarget = $(this).parent('.password-field');

            $outputTarget.removeClass(function (index, css) {
                return (css.match(/\level\S+/g) || []).join(' ');
            });

            $outputTarget.addClass('level' + strength);
        });
    });

    function checkPasswordStrength(password) {
        var strength = 0;

        // If password is 6 characters or longer
        if (password.length >= 6) {
            strength += 1;
        }

        // If password contains both lower and uppercase characters, increase strength value.
        if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) {
            strength += 1;
        }

        // If it has numbers and characters, increase strength value.
        if (password.match(/([a-zA-Z])/) && password.match(/([0-9])/)) {
            strength += 1;
        }

        return strength;
    }
    //var inputField = document.querySelectorAll('input');
    //if (inputField.length) {
    //    var mailValidator = /^[A-Z0-9._%+-]+@([A-Z0-9-]+\.)+[A-Z]{2,4}$/i;
    //    var phoneValidator = /^[(]{0,1}[0-9]{3}[)]{0,1}[-\s\.]{0,1}[0-9]{3}[-\s\.]{0,1}[0-9]{4}$/;
    //    var nameValidator = /^[a-zA-ZàáâäãåąčćęèéêëėįìíîïłńòóôöõøùúûüųūÿýżźñçčšžÀÁÂÄÃÅĄĆČĖĘÈÉÊËÌÍÎÏĮŁŃÒÓÔÖÕØÙÚÛÜŲŪŸÝŻŹÑßÇŒÆČŠŽ∂ð ,.'-]+$/u;
    //    var passwordValidator = /[A-Za-z]{2}[A-Za-z]*[ ]?[A-Za-z]*/;
    //    var numberValidator = /^(0|[1-9]\d*)$/;
    //    var linkValidator = /^(http|https)?:\/\/[a-zA-Z0-9-\.]+\.[a-z]{2,4}/;
    //    var textValidator = /[A-Za-z]{2}[A-Za-z]*[ ]?[A-Za-z]*/;

    //    function valid(el) {
    //        el.parentElement.querySelectorAll('.valid')[0].classList.remove('disabled');
    //        el.parentElement.querySelectorAll('.invalid')[0].classList.add('disabled');
    //    }
    //    function invalid(el) {
    //        el.parentElement.querySelectorAll('.valid')[0].classList.add('disabled');
    //        el.parentElement.querySelectorAll('.invalid')[0].classList.remove('disabled');
    //    }
    //    function unfilled(el) {
    //        el.parentElement.querySelectorAll('em')[0].classList.remove('disabled');
    //        el.parentElement.querySelectorAll('.valid')[0].classList.add('disabled');
    //        el.parentElement.querySelectorAll('.invalid')[0].classList.add('disabled');
    //    }

    //    var regularField = document.querySelectorAll('.input-style input:not([type="date"])')
    //    regularField.forEach(el => el.addEventListener('keyup', e => {
    //        if (!el.value == "") {
    //            el.parentElement.classList.add('input-style-active');
    //            el.parentElement.querySelector('em').classList.add('disabled');
    //        } else {
    //            el.parentElement.querySelectorAll('.valid')[0].classList.add('disabled');
    //            el.parentElement.querySelectorAll('.invalid')[0].classList.add('disabled');
    //            el.parentElement.classList.remove('input-style-active');
    //            el.parentElement.querySelector('em').classList.remove('disabled');
    //        }
    //    }));

    //    var regularTextarea = document.querySelectorAll('.input-style textarea')
    //    regularTextarea.forEach(el => el.addEventListener('keyup', e => {
    //        if (!el.value == "") {
    //            el.parentElement.classList.add('input-style-active');
    //            el.parentElement.querySelector('em').classList.add('disabled');
    //        } else {
    //            el.parentElement.classList.remove('input-style-active');
    //            el.parentElement.querySelector('em').classList.remove('disabled');
    //        }
    //    }));

    //    var selectField = document.querySelectorAll('.input-style select')
    //    selectField.forEach(el => el.addEventListener('change', e => {
    //        if (el.value !== "default") {
    //            el.parentElement.classList.add('input-style-active');
    //            el.parentElement.querySelectorAll('.valid')[0].classList.remove('disabled');
    //            el.parentElement.querySelectorAll('.invalid, em, span')[0].classList.add('disabled');
    //        }
    //        if (el.value == "default") {
    //            el.parentElement.querySelectorAll('span, .valid, em')[0].classList.add('disabled');
    //            el.parentElement.querySelectorAll('.invalid')[0].classList.remove('disabled');
    //            el.parentElement.classList.add('input-style-active');
    //        }
    //    }));

    //    var dateField = document.querySelectorAll('.input-style input[type="date"]')
    //    dateField.forEach(el => el.addEventListener('change', e => {
    //        el.parentElement.classList.add('input-style-active');
    //        el.parentElement.querySelectorAll('.valid')[0].classList.remove('disabled');
    //        el.parentElement.querySelectorAll('.invalid')[0].classList.add('disabled');
    //    }));

    //    var validateField = document.querySelectorAll('.validate-field input, .validator-field textarea');
    //    if (validateField.length) {
    //        validateField.forEach(el => el.addEventListener('keyup', e => {
    //            var getAttribute = el.getAttribute('type');
    //            switch (getAttribute) {
    //                case 'name': nameValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'number': numberValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'email': mailValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'text': textValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'url': linkValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'tel': phoneValidator.test(el.value) ? valid(el) : invalid(el); break;
    //                case 'password': passwordValidator.test(el.value) ? valid(el) : invalid(el); break;
    //            }
    //            if (el.value === "") { unfilled(el); }
    //        }));
    //    }
    //}
}