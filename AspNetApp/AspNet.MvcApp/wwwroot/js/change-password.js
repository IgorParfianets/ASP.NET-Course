let stateClick = true;

const trigger = document.getElementById("trigger");
const passBlock = document.getElementById("change-password-block");


trigger.onclick = function () {
    if (stateClick) {
        trigger.textContent = 'Оставить прежним'

        passBlock.style.transform = 'scaleY(1)'
        passBlock.style.height = '160px'
        passBlock.style.paddingTop = '15px'

        stateClick = false;
    } else {
        trigger.textContent = 'Сменить пароль'

        passBlock.style.transform = 'scaleY(0)'
        passBlock.style.height = '0px'   
        
        stateClick = true;
    }
}