// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
     // Show a spinner only after the normal field validators accept the form.
        (() => {
            const form = document.getElementById("postCreateForm");
            const button = document.getElementById("postCreateSubmit");
            const label = button?.querySelector(".post-create-submit-label");

            if (!form || !button || !label) {
                return;
            }

            form.addEventListener("submit", () => {
                const jqueryForm = window.jQuery ? window.jQuery(form) : null;
                if (jqueryForm && typeof jqueryForm.valid === "function" && !jqueryForm.valid()) {
                    return;
                }

                button.disabled = true;
                button.setAttribute("aria-busy", "true");
                button.classList.add("is-submitting");
                label.textContent = "Uploading image...";
            });
        })();


