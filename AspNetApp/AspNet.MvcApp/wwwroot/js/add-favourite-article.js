let state = false;


document.getElementById("favourite").onclick = function () {
    if (stateClick) {
        document.getElementById("star").src = "/image/star_disable.png";
        stateClick = false;
    } else {
        document.getElementById("star").src = "/image/star_funny.png";
        stateClick = true;
    }
}
