const apiurl =
    window.location.hostname === "localhost"
        ? "https://localhost:44385"
        : "https://192.168.1.106:4432";


document.getElementById("registerForm")
    .addEventListener("submit", function (e) {

        e.preventDefault();

        const firstName = document.getElementById("firstName").value.trim();
        const lastName = document.getElementById("lastName").value.trim();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value;
        const mobile = document.getElementById("mobile").value.trim();
        const role = document.getElementById("role").value;

        if (!role) {
            alert("Please select a role");
            return;
        }

        //const names = fullName.split(" ");

        const payload = {
            registerId: 0,
            firstName: firstName,
            lastName: lastName,
            email: email,
            password: password,
            mobile: mobile,
            role: parseInt(role),
            isActive: true
        };

        fetch(`${apiurl}/api/register/insert-update`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        })
            .then(async response => {

                if (!response.ok) {
                    const err = await response.json();
                    alert(err.message || "Registration failed");
                    return;
                }

                return response.json();
            })
            .then(result => {
                if (!result) return;

                alert(result.message);
                window.location.href = "/Home/Index";
            })
            .catch(err => {
                console.error(err);
                alert("Network error");
            });
    });

function togglePassword() {
    const passwordInput = document.getElementById("password");
    const eyeIcon = document.getElementById("eyeIcon");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
        eyeIcon.classList.remove("fa-eye");
        eyeIcon.classList.add("fa-eye-slash");
    } else {
        passwordInput.type = "password";
        eyeIcon.classList.remove("fa-eye-slash");
        eyeIcon.classList.add("fa-eye");
    }
}
