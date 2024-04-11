// arrow
function arrow() {
    const p = document.querySelectorAll(".left-panel p");
    const icon = document.querySelectorAll(".icon-arrow i");
    p.forEach((item) => {
        item.classList.toggle("none");
    })
    icon.forEach((item) => {
        item.classList.toggle("none");
    })
}