const input = document.getElementById("file");
const spanPlus = document.getElementById("plus")
const image = document.getElementById("ava")


spanPlus.addEventListener("click", () => {
    input.click();
});

input.addEventListener("change", (event) => {

    if (!event.target.files.length) {
        return;
    }

    const files = Array.from(event.target.files);

    files.forEach((file) => {

        if (!file.type.match("image")) return;

        const reader = new FileReader();
        
        reader.onload = (e) => {
            image.src = e.target.result
        };

        reader.readAsDataURL(file);
    });
});