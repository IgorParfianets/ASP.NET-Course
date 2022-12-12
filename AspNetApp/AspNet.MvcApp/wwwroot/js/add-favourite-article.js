let answer = false
const urlAdd = `${window.location.origin}/Account/AddFavourite`;
const urlCheck = `${window.location.origin}/Account/CheckFavourite`;

document.addEventListener('DOMContentLoaded', async function () {

    let artId = document.forms.test[0]['defaultValue']

    let data = {
        articleId: artId
    }

    exist = await checkData(urlCheck, data) // toString text
    answer = exist["exist"]

    if (answer) {
        document.getElementById("star").src = "/image/star_funny.png";
    } else {
        document.getElementById("star").src = "/image/star_disable.png";
    }
}, { once: true })

document.getElementById("favourite").addEventListener('click', async function () {

    let artId = document.forms.test[0]['defaultValue']
    
    if (answer) {
        document.getElementById("star").src = "/image/star_disable.png";

        let data = {
            answer: false,
            articleId: artId
        }
        await postData(urlAdd, data)

        answer = false
        alert('Удалено из избранного!', 'warning')
    } else {
        document.getElementById("star").src = "/image/star_funny.png";

        let data = {
            answer: true,
            articleId: artId
        }
        await postData(urlAdd, data)

        answer = true;
        alert('Добавлено в избранное!', 'success')
    }
})

const alert = (message, type) => {
    const popMessage = document.getElementById("pop-message")
    const wrapper = document.createElement('div')

    if (popMessage.hasChildNodes)
        popMessage.removeChild(popMessage.firstChild)

    //wrapper.innerHTML = [
    //    `<div class="alert alert-${type} fade show" role="alert">`,
    //    `${message}`,
    //    '</div>'
    //].join('')
    wrapper.innerHTML = message

    wrapper.classList.add('alert')
    wrapper.classList.add(`alert-${type}`)
    wrapper.classList.add('message') 

    popMessage.insertBefore(wrapper, popMessage.firstChild)
}

async function checkData(url, data) {
    const response = await fetch(url, {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer',
        body: JSON.stringify(data)
    });
    return await response.json();
}

async function postData(url, data) {
    await fetch(url, {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        credentials: 'same-origin',
        headers: {
            'Content-Type': 'application/json; charset=utf-8'
        },
        redirect: 'follow',
        referrerPolicy: 'no-referrer',
        body: JSON.stringify(data)
    });
}



