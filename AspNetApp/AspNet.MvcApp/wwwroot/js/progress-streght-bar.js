let state = false; // for toggle
let password = document.getElementById("password");


let passwordStrength = document.getElementById("password-strength");
let problem = document.getElementById("test");

//let lowUpperCase = document.querySelector(".low-upper-case i");
//let number = document.querySelector(".one-number i");
//let specialChar = document.querySelector(".one-special-char i");
//let eightChar = document.querySelector(".eight-character i");
//let fiveChar = document.querySelector(".five-character i");




password.addEventListener("keyup", function () {

    let pass = document.getElementById("password").value;
    checkStrength(pass);
});


function myFunction(show) {
    show.classList.toggle("fa-eye-slash");
}

function checkStrength(password) {
    let strength = 0;

    //If password contains both lower and uppercase characters
    if (password.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)) {
        strength += 1;
    }

    //If it has numbers and characters
    if (password.match(/([0-9])/)) {
        strength += 1;
    }

    //If it has one special character
    if (password.match(/([!,%,&,@,#,$,^,*,?,_,~])/)) {
        strength += 1;
    }

    //If password is greater than 7
    if (password.length > 8) {
        strength += 1;

    }


        problem.textContent = "Минимум 4 символа";
        problem.style.color = "#6C7073";
    
    // If value is less than 2
    if (strength == 1) {

        problem.textContent = "Слабый";
        problem.style.color = "#e90f10";
        passwordStrength.classList.remove("progress-bar-warning");
        passwordStrength.classList.remove("progress-bar-success");
        passwordStrength.classList.remove("progress-bar-great");
        passwordStrength.classList.add("progress-bar-danger");
        passwordStrength.style = "width: 25%";
    } else if (strength == 2) {

        problem.textContent = "Средний";
        problem.style.color = "#FF8C00";
        passwordStrength.classList.remove("progress-bar-success");
        passwordStrength.classList.remove("progress-bar-danger");
        passwordStrength.classList.remove("progress-bar-great");
        passwordStrength.classList.add("progress-bar-warning");
        passwordStrength.style = "width: 50%";
    } else if (strength == 3) {

        problem.textContent = "Сильный";
        problem.style.color = "#40E0D0";
        passwordStrength.classList.remove("progress-bar-warning");
        passwordStrength.classList.remove("progress-bar-danger");
        passwordStrength.classList.remove("progress-bar-great");
        passwordStrength.classList.add("progress-bar-success");
        passwordStrength.style = "width: 75%";
    } else if (strength == 4) {

        problem.textContent = "Очень сильный";
        problem.style.color = "#00ef00";
        passwordStrength.classList.remove("progress-bar-warning");
        passwordStrength.classList.remove("progress-bar-danger");
        passwordStrength.classList.remove("progress-bar-success");
        passwordStrength.classList.add("progress-bar-great");
        passwordStrength.style = "width: 100%";
    }


}

function toggle() {
    if (state) {
        document.getElementById("password").setAttribute("type", "password");
        state = false;
    } else {
        document.getElementById("password").setAttribute("type", "text");
        state = true;
    }
}