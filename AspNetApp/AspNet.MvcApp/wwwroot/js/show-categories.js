const url = `${window.location.origin}/Article/GetAllCategories`;
let categoryArray = [];

function getCategories() {
    fetch(url)
        .then(response => response.json())
        .then(result => {
            result.forEach(category => categoryArray.push(category)).
            renderCharacterList(categoryArray)
        }).catch(() => console.error('smth goes wrong')
        );


     
    
}