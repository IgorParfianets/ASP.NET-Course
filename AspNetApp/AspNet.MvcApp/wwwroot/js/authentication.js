const navBar = document.getElementById('user-info');

const url = `${window.location.origin}/Account/LoginLogoutPreview`; // todo rework incorrect working 

fetch(url)
    .then(function(response) {
        return response.text();
    }).then(function(result) {
        navBar.innerHTML = result;
    }).catch(function() {
        console.Error('Error');
    });

