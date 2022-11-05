let state = false; // for toggle
let password = document.getElementById("password");
let passwordStrength = document.getElementById("password-strength");

let lowUpperCase = document.querySelector(".low-upper-case i");
let number = document.querySelector(".one-number i");
let specialChar = document.querySelector(".one-special-char i");
let eightChar = document.querySelector(".eight-character i");
let fiveChar = document.querySelector(".five-character i");

password.addEventListener("keyup", function () {
    let pass = document.getElementById("password").value;
    checkStrength(pass);
});

function toggle() {
    if (state) {
        document.getElementById("password").setAttribute("type", "password");
        state = false;
    } else {
        document.getElementById("password").setAttribute("type", "text");
        state = true;
    }
}

function myFunction(show) {
    show.classList.toggle("fa-eye-slash");
}

function checkStrength(password) {
    let strength = 0;

    if (password.length > 4) {
        //If password contains both lower and uppercase characters
        fiveChar.classList.remove("fa-circle");
        fiveChar.classList.add("fa-check");
        if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) {
            strength += 1;
            lowUpperCase.classList.remove("fa-circle");
            lowUpperCase.classList.add("fa-check");
        } else {
            lowUpperCase.classList.add("fa-circle");
            lowUpperCase.classList.remove("fa-check");
        }
        //If it has numbers and characters
        if (password.match(/([0-9])/)) {
            strength += 1;
            number.classList.remove("fa-circle");
            number.classList.add("fa-check");
        } else {
            number.classList.add("fa-circle");
            number.classList.remove("fa-check");
        }
        //If it has one special character
        if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) {
            strength += 1;
            specialChar.classList.remove("fa-circle");
            specialChar.classList.add("fa-check");
        } else {
            specialChar.classList.add("fa-circle");
            specialChar.classList.remove("fa-check");
        }
        //If password is greater than 7
        if (password.length > 9) {
            strength += 1;
            eightChar.classList.remove("fa-circle");
            eightChar.classList.add("fa-check");
        } else {
            eightChar.classList.add("fa-circle");
            eightChar.classList.remove("fa-check");
        }
        // If value is less than 2
        if (strength == 1) {
            passwordStrength.classList.remove("progress-bar-warning");
            passwordStrength.classList.remove("progress-bar-success");
            passwordStrength.classList.remove("progress-bar-great");
            passwordStrength.classList.add("progress-bar-danger");
            passwordStrength.style = "width: 25%";
        } else if (strength == 2) {
            passwordStrength.classList.remove("progress-bar-success");
            passwordStrength.classList.remove("progress-bar-danger");
            passwordStrength.classList.remove("progress-bar-great");
            passwordStrength.classList.add("progress-bar-warning");
            passwordStrength.style = "width: 50%";
        } else if (strength == 3) {
            passwordStrength.classList.remove("progress-bar-warning");
            passwordStrength.classList.remove("progress-bar-danger");
            passwordStrength.classList.remove("progress-bar-great");
            passwordStrength.classList.add("progress-bar-success");
            passwordStrength.style = "width: 75%";
        } else if (strength == 4) {
            passwordStrength.classList.remove("progress-bar-warning");
            passwordStrength.classList.remove("progress-bar-danger");
            passwordStrength.classList.remove("progress-bar-success");
            passwordStrength.classList.add("progress-bar-great");
            passwordStrength.style = "width: 100%";
        }
    } else {
        strength == 0;
        lowUpperCase.classList.remove("fa-check");
        number.classList.remove("fa-check");
        specialChar.classList.remove("fa-check");
        eightChar.classList.remove("fa-check");
        fiveChar.classList.remove("fa-check");

        lowUpperCase.classList.add("fa-circle");
        number.classList.add("fa-circle");
        specialChar.classList.add("fa-circle");
        eightChar.classList.add("fa-circle");
        fiveChar.classList.add("fa-circle");

        passwordStrength.classList.remove("progress-bar-warning");
        passwordStrength.classList.remove("progress-bar-success");
        passwordStrength.classList.remove("progress-bar-great");
        passwordStrength.classList.remove("progress-bar-danger");
        passwordStrength.style = "width: 0%";
    }
}
