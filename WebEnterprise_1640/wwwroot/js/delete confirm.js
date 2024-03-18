function deletel() {
    const show = document.querySelector(".show-modal");
    show.classList.toggle("none");
}

function closemodal() {
    const show = document.querySelector(".show-modal");
    show.classList.toggle("none");
}

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