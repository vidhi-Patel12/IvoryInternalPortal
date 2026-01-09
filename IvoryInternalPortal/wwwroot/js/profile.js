const apiurl =
    window.location.hostname === "localhost"
        ? "https://localhost:44385"
        : "https://192.168.1.106:4432";

let profileTable;
let profileModal;
let deleteModal;
let allProfiles = [];
let filteredProfiles = [];
let currentPage = 1;
const pageSize = 10;

   //OPEN CREATE MODAL

window.openCreateModal = function () {

    document.getElementById("profileForm").reset();
    document.getElementById("ProfileId").value = 0;
    document.getElementById("modalTitle").innerText = "Add Profile";

    document.getElementById("projectsContainer").innerHTML = "";
    addProject();

    profileModal = new bootstrap.Modal(
        document.getElementById("profileModal")
    );
    profileModal.show();
};

   //ADD PROJECT

window.addProject = function () {

    const container = document.getElementById("projectsContainer");
    const count = container.children.length;

    if (count >= maxProjects) {
        alert("Maximum 8 projects allowed");
        return;
    }

    const index = count + 1;

    const html = `
        <div class="project-row border rounded p-3 mb-3">

            <h6 class="fw-bold">Project ${index}</h6>

            <input type="text" class="form-control mb-2 project-name"
                   placeholder="Project Name">

            <input type="text" class="form-control mb-2 project-domain"
                   placeholder="Domain">

            <textarea class="form-control mb-2 project-tech"
                      rows="2"
                      placeholder="Technologies (comma separated)"></textarea>

            <textarea class="form-control mb-2 project-desc"
                      rows="3"
                      placeholder="Project Description"></textarea>

            <textarea class="form-control mb-2 project-resp"
                      rows="3"
                      placeholder="Responsibilities (use | for bullets)"></textarea>

            <button type="button"
                    class="btn btn-sm btn-outline-danger"
                    onclick="removeProject(this)">
                Remove Project
            </button>
        </div>
    `;

    container.insertAdjacentHTML("beforeend", html);
};


   //REMOVE PROJECT
window.removeProject = function (btn) {
    btn.closest(".project-row").remove();
};


   //LOAD PROFILES

//function loadProfiles() {

//    fetch(`${apiurl}/api/profile/get-all`)
//        .then(res => res.json())
//        .then(result => {

//            if (!result.success) {
//                alert("Failed to load profiles");
//                return;
//            }

//            let rows = "";

//            result.data.forEach(p => {
//                rows += `
//                    <tr>
//                        <td>${p.name}</td>
//                        <td>${p.coreSkills ?? ""}</td>
//                        <td>
//                            <button class="btn btn-sm btn-outline-info action-btn"
//                                    onclick="openViewModal(${p.profileId})">
//                                <i class="bi bi-eye"></i>
//                            </button>

//                            <button class="btn btn-sm btn-outline-primary action-btn"
//                                    onclick='openEditModal(${JSON.stringify(p)})'>
//                                <i class="bi bi-pencil-square"></i>
//                            </button>

//                            <button class="btn btn-sm btn-outline-danger action-btn"
//                                    onclick="openDeleteModal(${p.profileId})">
//                                <i class="bi bi-trash"></i>
//                            </button>

//                            <button class="btn btn-sm btn-outline-success action-btn"
//                                    onclick="downloadPdf(${p.profileId})">
//                                <i class="bi bi-file-earmark-pdf"></i>
//                            </button>
//                        </td>
//                    </tr>
//                `;
//            });

//            document.getElementById("profileTableBody").innerHTML = rows;

//            if ($.fn.DataTable.isDataTable('#profileTable')) {
//                $('#profileTable').DataTable().destroy();
//            }

//            profileTable = $('#profileTable').DataTable({
//                pageLength: 5,
//                responsive: true,
//                ordering: true,
//                columnDefs: [{ orderable: false, targets: 2 }]
//            });
//        })
//        .catch(err => console.error(err));
//}


function loadProfiles() {
    fetch(`${apiurl}/api/profile/get-all`)
        .then(res => res.json())
        .then(result => {

            if (!result.success) {
                alert("Failed to load profiles");
                return;
            }

            allProfiles = result.data;
            filteredProfiles = [...allProfiles];
            currentPage = 1;

            renderProfiles();
            updatePagination();
        })
        .catch (err => console.error(err));
}

function renderProfiles() {
    const container = document.getElementById("profileGrid");
    container.innerHTML = "";

    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    const pageItems = filteredProfiles.slice(start, end);

    pageItems.forEach(p => {

        const initials = p.name
            .split(" ")
            .map(x => x[0])
            .join("")
            .substring(0, 2)
            .toUpperCase();

        container.innerHTML += `
        <div class="col-md-4">
            <div class="profile-card">

                <div class="profile-actions">
                    <i class="bi bi-eye text-info"
                       onclick="openViewModal(${p.profileId})"></i>
                    <i class="bi bi-pencil-square text-primary"
                       onclick='openEditModal(${JSON.stringify(p)})'></i>
                    <i class="bi bi-trash text-danger"
                       onclick="openDeleteModal(${p.profileId})"></i>
                    <i class="bi bi-file-earmark-pdf text-success"
                       onclick="downloadPdf(${p.profileId})"></i>
                </div>

                <div class="d-flex gap-3">
                   <!-- <div class="avatar">${initials}</div> -->
                    <div>
                        <div class="profile-name">${p.name}</div>
                        <div class="profile-skills" title="${p.coreSkills ?? ''}">
                            ${p.coreSkills ?? ""}
                        </div>
                    </div>
                </div>

            </div>
        </div>`;
    });
}


function updatePagination() {

    const totalPages =
        Math.ceil(filteredProfiles.length / pageSize);

    document.getElementById("pageInfo").innerText =
        `Page ${currentPage} of ${totalPages || 1}`;

    document.getElementById("prevBtn").disabled =
        currentPage === 1;

    document.getElementById("nextBtn").disabled =
        currentPage === totalPages || totalPages === 0;
}

document.getElementById("prevBtn").onclick = () => {
    if (currentPage > 1) {
        currentPage--;
        renderProfiles();
        updatePagination();
    }
};

document.getElementById("nextBtn").onclick = () => {
    const totalPages =
        Math.ceil(filteredProfiles.length / pageSize);

    if (currentPage < totalPages) {
        currentPage++;
        renderProfiles();
        updatePagination();
    }
};

document.getElementById("pageSize")
    .addEventListener("change", function () {

        pageSize = parseInt(this.value);
        currentPage = 1;

        renderProfiles();
        updatePagination();
    });


document.getElementById("searchInput").addEventListener("input", function () {

    const query = this.value.toLowerCase().trim();

    filteredProfiles = allProfiles.filter(p => {
        return (
            p.name?.toLowerCase().includes(query) ||
            p.coreSkills?.toLowerCase().includes(query) ||
            p.professionalSummary?.toLowerCase().includes(query)
        )
    });

    currentPage = 1;
    renderProfiles();
    updatePagination();
});


  // DELETE

window.openDeleteModal = function (id) {
    document.getElementById("deleteProfileId").value = id;
    deleteModal = new bootstrap.Modal(document.getElementById("deleteModal"));
    deleteModal.show();
};

window.confirmDelete = function () {

    const id = document.getElementById("deleteProfileId").value;

    fetch(`${apiurl}/api/profile/${id}`, { method: "DELETE" })
        .then(res => res.json())
        .then(result => {
            if (!result.success) {
                alert(result.message);
                return;
            }

            alert(result.message);
            deleteModal.hide();
            loadProfiles();
        });
};


  // VIEW PROFILE

window.openViewModal = function (id) {

    fetch(`${apiurl}/api/profile/${id}`)
        .then(res => res.json())
        .then(result => {

            const p = result.data;

            let projectHtml = "";

            const names = (p.projectNames || "").split("[PROJECT]");
            const domains = (p.projectDomains || "").split("[PROJECT]");
            const techs = (p.projectTechnologies || "").split("[PROJECT]");

            names.forEach((n, i) => {
                projectHtml += `
                    <div class="mb-2">
                        <strong>${n}</strong><br/>
                        <small>Domain: ${domains[i] || ""}</small><br/>
                        <small>Tech: ${techs[i] || ""}</small>
                    </div>
                `;
            });

            document.getElementById("viewProfileBody").innerHTML = `
                <tr><th>Name</th><td>${p.name}</td></tr>
                <tr><th>Summary</th><td>${p.professionalSummary ?? ""}</td></tr>
                <tr><th>Core Skills</th><td>${p.coreSkills ?? ""}</td></tr>
                <tr><th>Projects</th><td>${projectHtml}</td></tr>
            `;

            new bootstrap.Modal(
                document.getElementById("viewProfileModal")
            ).show();
        });
};


  // EDIT PROFILE
window.openEditModal = function (p) {

    document.getElementById("modalTitle").innerText = "Edit Profile";
    document.getElementById("ProfileId").value = p.profileId;

    document.querySelector("[name='Name']").value = p.name;
    document.querySelector("[name='ProfessionalSummary']").value = p.professionalSummary ?? "";
    document.querySelector("[name='CoreSkills']").value = p.coreSkills ?? "";
    document.querySelector("[name='ProfessionalExperience']").value = p.professionalExperience ?? "";
    document.querySelector("[name='KeySkills']").value = p.keySkills ?? "";

    const container = document.getElementById("projectsContainer");
    container.innerHTML = "";

    const names = (p.projectNames || "").split("[PROJECT]");
    const domains = (p.projectDomains || "").split("[PROJECT]");
    const techs = (p.projectTechnologies || "").split("[PROJECT]");
    const descs = (p.projectDescriptions || "").split("[PROJECT]");
    const resps = (p.projectResponsibilities || "").split("[PROJECT]");

    for (let i = 0; i < names.length; i++) {
        addProject();
        const row = container.lastElementChild;

        row.querySelector(".project-name").value = names[i] || "";
        row.querySelector(".project-domain").value = domains[i] || "";
        row.querySelector(".project-tech").value = techs[i] || "";
        row.querySelector(".project-desc").value = descs[i] || "";
        row.querySelector(".project-resp").value = resps[i] || "";
    }

    profileModal = new bootstrap.Modal(
        document.getElementById("profileModal")
    );
    profileModal.show();
};


  // FORM SUBMIT
document.addEventListener("DOMContentLoaded", function () {

    loadProfiles();

    document.getElementById("profileForm").addEventListener("submit", function (e) {
        e.preventDefault();

        const form = document.getElementById("profileForm");

        const names = [], domains = [], techs = [], descs = [], resps = [];

        document.querySelectorAll(".project-row").forEach(row => {
            names.push(row.querySelector(".project-name").value);
            domains.push(row.querySelector(".project-domain").value);
            techs.push(row.querySelector(".project-tech").value);
            descs.push(row.querySelector(".project-desc").value);
            resps.push(row.querySelector(".project-resp").value);
        });

        const payload = {
            ProfileId: form.ProfileId.value,
            Name: form.Name.value,
            ProfessionalSummary: form.ProfessionalSummary.value,
            CoreSkills: form.CoreSkills.value,
            ProfessionalExperience: form.ProfessionalExperience.value,

            ProjectNames: names.join("[PROJECT]"),
            ProjectDomains: domains.join("[PROJECT]"),
            ProjectTechnologies: techs.join("[PROJECT]"),
            ProjectDescriptions: descs.join("[PROJECT]"),
            ProjectResponsibilities: resps.join("[PROJECT]"),

            KeySkills: form.KeySkills.value
        };

        fetch(`${apiurl}/api/profile/insert-update`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        })
            .then(res => res.json())
            .then(result => {
                alert(result.message);
                profileModal.hide();
                window.location.reload();
                loadProfiles();
            });
    });
});


  // PDF
window.downloadPdf = function (id) {
    window.open(`${apiurl}/api/profile/generate-pdf/${id}`, "_blank");
};
