
const apiurl =
    window.location.hostname === "localhost"
        ? "https://localhost:44385"
        : "https://192.168.1.106:4432";

let employeeTable;
let employeeModal;
let allEmployees = [];
let filteredEmployees = [];
let currentPage = 1;
const pageSize = 10;


// OPEN CREATE
window.openCreateModal = function () {
    document.getElementById("employeeForm").reset();
    document.getElementById("EmployeeId").value = 0;
    document.getElementById("modalTitle").innerText = "Add Employee";

    resetFileDisplays();

    employeeModal = new bootstrap.Modal(
        document.getElementById("employeeModal")
    );
    employeeModal.show();
    document
        .getElementById("employeeModal")
        .addEventListener("shown.bs.modal", initSignaturePad, { once: true });

    //setTimeout(initSignaturePad, 300);
};

function resetFileDisplays() {
    const displays = [
        "profileFileDisplay",
        "aadhaarFileDisplay",
        "panFileDisplay",
        "agreementFileDisplay",
        "signatureFileDisplay"
    ];

    displays.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.innerText = "No file chosen";
    });

    // Clear file inputs
    const inputs = [
        "uploadProfile",
        "aadhaarFile",
        "panFile",
        "agreementFile",
        "signatureFile"
    ];

    inputs.forEach(name => {
        const input = document.querySelector(`[name='${name}']`);
        if (input) input.value = "";
    });
}


// OPEN EDIT
window.openEditModal = function (emp) {

    document.getElementById("modalTitle").innerText = "Edit Employee";

    document.querySelector("[name='EmployeeId']").value = emp.employeeId;
    document.querySelector("[name='EmployeeName']").value = emp.employeeName;
    document.querySelector("[name='EmployeeEmail']").value = emp.employeeEmail || "";
    document.querySelector("[name='EmployeePhone']").value = emp.employeePhone || "";
    document.querySelector("[name='EmployeeAddress']").value = emp.employeeAddress || "";
    document.querySelector("[name='Designation']").value = emp.designation || "";
    document.querySelector("[name='Department']").value = emp.department || "";
    document.querySelector("[name='EmploymentType']").value = emp.employmentType || "";
    document.querySelector("[name='Skills']").value = emp.skills || "";
    document.querySelector("[name='BankName']").value = emp.bankName || "";
    document.querySelector("[name='AccountNumber']").value = emp.accountNumber || "";
    document.querySelector("[name='IFSCCode']").value = emp.ifscCode || "";

    document.querySelector("[name='AccountHolderName']").value = emp.accountHolderName || "";
    document.querySelector("[name='AadhaarNumber']").value = emp.aadhaarNumber || "";
    document.querySelector("[name='PANNumber']").value = emp.panNumber || "";
    document.querySelector("[name='Salary']").value = emp.salary || "";
    document.querySelector("[name='PayrollCycle']").value = emp.payrollCycle || "";


    setExistingProfileFile(emp.uploadProfile);

    setExistingAadhaarFile(emp.aadhaarDocumentPath);

    setExistingPanFile(emp.panDocumentPath);


    setExistingAgreementFile(emp.agreementPdfPath);

    setExistingSignatureFile(emp.digitalSignaturePath);


    employeeModal = new bootstrap.Modal(
        document.getElementById("employeeModal")
    );
    employeeModal.show();
    setTimeout(initSignaturePad, 300);

};

function setExistingProfileFile(filePath) {
    const display = document.getElementById("profileFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function setExistingAadhaarFile(filePath) {
    const display = document.getElementById("aadhaarFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function setExistingPanFile(filePath) {
    const display = document.getElementById("panFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function setExistingAgreementFile(filePath) {
    const display = document.getElementById("agreementFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}

function setExistingSignatureFile(filePath) {
    const display = document.getElementById("signatureFileDisplay");
    if (!display) return;

    if (filePath) {
        const fileName = filePath.split('/').pop();
        display.innerHTML = `
            ${fileName}
            <a href="${filePath}" target="_blank"
               class="ms-2 text-primary text-decoration-none">(View)</a>`;
    } else {
        display.innerText = "No file chosen";
    }
}


function onProfileFileChange(input) {
    const display = document.getElementById("profileFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;
    } else {
        display.innerText = "No file chosen";
    }
}

function onAadhaarFileChange(input) {
    const display = document.getElementById("aadhaarFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;
    } else {
        display.innerText = "No file chosen";
    }
}

function onPanFileChange(input) {
    const display = document.getElementById("panFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;
    } else {
        display.innerText = "No file chosen";
    }
}
function onAgreementFileChange(input) {
    const display = document.getElementById("agreementFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;
    } else {
        display.innerText = "No file chosen";
    }
}

function onSignatureFileChange(input) {
    const display = document.getElementById("signatureFileDisplay");
    if (!display) return;

    if (input.files && input.files.length > 0) {
        display.innerText = input.files[0].name;

        // ❗ Clear canvas signature if file selected
        clearPad();
        document.getElementById("signatureData").value = "";
    } else {
        display.innerText = "No file chosen";
    }
}


// LOAD EMPLOYEES
//function loadEmployees() {
//    fetch(`${apiurl}/api/employee/get-all`)
//        .then(res => res.json())
//        .then(result => {

//            if (!result.success) {
//                alert("Failed to load employees");
//                return;
//            }

//            let rows = "";

//            result.data.forEach(emp => {
//                rows += `
//                <tr>
//                    <td>${emp.employeeName}</td>
//                    <td>${emp.employeeEmail ?? ""}</td>
//                    <td>${emp.employeePhone ?? ""}</td>
//                    <td>${emp.designation ?? ""}</td>
//                    <td style="width: 145px;">
//                       <button class="btn btn-sm btn-outline-info action-btn"
//                                onclick="openViewModal(${emp.employeeId})">
//                            <i class="bi bi-eye"></i>
//                        </button>

//                        <button class="btn btn-sm btn-outline-primary action-btn"
//                                onclick='openEditModal(${JSON.stringify(emp)})'>
//                            <i class="bi bi-pencil-square"></i>
//                        </button>

//                        <button class="btn btn-sm btn-outline-danger action-btn"
//                                onclick="openDeleteModal(${emp.employeeId})">
//                            <i class="bi bi-trash"></i>
//                        </button>

//                    </td>
//                </tr>`;
//            });

//            document.getElementById("employeeTableBody").innerHTML = rows;

//            // Destroy existing DataTable before reinit
//            if ($.fn.DataTable.isDataTable('#employeeTable')) {
//                $('#employeeTable').DataTable().destroy();
//            }

//            // Initialize DataTable
//            employeeTable = $('#employeeTable').DataTable({
//                pageLength: 5,
//                lengthMenu: [5, 10, 25, 50],
//                ordering: true,
//                searching: true,
//                responsive: true,
//                columnDefs: [
//                    { orderable: false, targets: 4 } // Disable sort on Action column
//                ]
//            });
//        })
//        .catch(err => console.error(err));
//}

// FORM SUBMIT (ADD / EDIT)


function loadEmployees() {

    fetch(`${apiurl}/api/employee/get-all`)
        .then(res => res.json())
        .then(result => {

            if (!result.success) {
                alert("Failed to load employees");
                return;
            }

            allEmployees = result.data;
            filteredEmployees = [...allEmployees];
            currentPage = 1;

            renderEmployees();
            updatePagination();
        })
        .catch(err => console.error(err));
}

function renderEmployees() {

    const container = document.getElementById("employeeCardContainer");
    container.innerHTML = "";

    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    const pageItems = filteredEmployees.slice(start, end);

    pageItems.forEach(emp => {

        const initials = emp.employeeName
            .split(" ")
            .map(x => x[0])
            .join("")
            .substring(0, 2)
            .toUpperCase();

        container.innerHTML += `
        <div class="col-12 col-md-6 col-lg-4">

            <div class="employee-card">

                <div class="card-actions">
                    <i class="bi bi-eye text-info"
                       onclick="openViewModal(${emp.employeeId})"></i>

                    <i class="bi bi-pencil-square text-primary"
                       onclick='openEditModal(${JSON.stringify(emp)})'></i>

                    <i class="bi bi-trash text-danger"
                       onclick="openDeleteModal(${emp.employeeId})"></i>
                </div>

                <div class="d-flex gap-3 align-items-center">
                    <div class="avatar">${initials}</div>
                    <div>
                        <h5 class="mb-0">${emp.employeeName}</h5>
                        <small class="text-muted">${emp.designation ?? ""}</small>
                    </div>
                </div>

                <span class="badge-active mt-2 d-inline-block">Active</span>

                <hr/>

                <div class="small mb-1">
                    <i class="bi bi-briefcase"></i>
                    ${emp.department ?? "—"}
                </div>

                <div class="small mb-1">
                    <i class="bi bi-envelope"></i>
                    ${emp.employeeEmail ?? "—"}
                </div>

                <div class="small mb-1">
                    <i class="bi bi-telephone"></i>
                    ${emp.employeePhone ?? "—"}
                </div>

                <hr/>

                <div class="small text-muted">
                    Salary<br/>
                    <strong>₹${emp.salary ?? "0"}</strong>
                </div>

            </div>
        </div>`;
    });
}

function updatePagination() {

    const totalPages =
        Math.ceil(filteredEmployees.length / pageSize);

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
        renderEmployees();
        updatePagination();
    }
};

document.getElementById("nextBtn").onclick = () => {
    const totalPages =
        Math.ceil(filteredEmployees.length / pageSize);

    if (currentPage < totalPages) {
        currentPage++;
        renderEmployees();
        updatePagination();
    }
};

document.getElementById("pageSize")
    .addEventListener("change", function () {

        pageSize = parseInt(this.value);
        currentPage = 1;

        renderEmployees();
        updatePagination();
    });




document.getElementById("searchInput").addEventListener("input", function () {

    const query = this.value.toLowerCase().trim();

    filteredEmployees = allEmployees.filter(emp => {
        return (
            emp.employeeName?.toLowerCase().includes(query) ||
            emp.employeeEmail?.toLowerCase().includes(query) ||
            emp.department?.toLowerCase().includes(query) ||
            emp.designation?.toLowerCase().includes(query)
        );
    });

    currentPage = 1;
    renderEmployees();
    updatePagination();
});


document.addEventListener("DOMContentLoaded", function () {

    loadEmployees();

    document.getElementById("employeeForm").addEventListener("submit", function (e) {
            e.preventDefault();

            const form = document.getElementById("employeeForm");
            const formData = new FormData(form);

            fetch(`${apiurl}/api/employee/insert-update`, {
                method: "POST",
                body: formData
            })
                .then(async response => {

                    // ❌ API error (400 / 500)
                    if (!response.ok) {
                        const error = await response.json();
                        alert(error.message || error.title || "Something went wrong");
                        return;
                    }

                    // ✅ API success
                    const res = await response.json();
                    alert(res.message);

                    bootstrap.Modal.getInstance(
                        document.getElementById("employeeModal")
                    ).hide();
                    window.location.reload();

                    loadEmployees();
                })
                .catch(err => {
                    console.error(err);
                    alert("Network error");
                });
    });


    let deleteModal;

    // OPEN DELETE MODAL
    window.openDeleteModal = function (id) {
        document.getElementById("deleteEmployeeId").value = id;

        deleteModal = new bootstrap.Modal(
            document.getElementById("deleteModal")
        );
        deleteModal.show();
    };

    // CONFIRM DELETE
    window.confirmDelete = function () {
        const id = document.getElementById("deleteEmployeeId").value;

        fetch(`${apiurl}/api/employee/${id}`, {
            method: "DELETE"
        })
            .then(res => res.json())
            .then(result => {
                if (!result.success) {
                    alert(result.message || "Delete failed");
                    return;
                }

                alert(result.message);
                deleteModal.hide();
                loadEmployees();
            })
            .catch(err => {
                console.error(err);
                alert("Network error");
            });
    };

    window.openViewModal = function (employeeId) {

        fetch(`${apiurl}/api/employee/${employeeId}`)
            .then(res => {
                if (!res.ok) throw new Error("Employee not found");
                return res.json();
            })
            .then(result => {

                const emp = result.data;

                let html = `
                <tr><th>Name</th><td>${emp.employeeName}</td></tr>
                <tr><th>Email</th><td>${emp.employeeEmail ?? ""}</td></tr>
                <tr><th>Phone</th><td>${emp.employeePhone ?? ""}</td></tr>
                <tr><th>Address</th><td>${emp.employeeAddress ?? ""}</td></tr>
                <tr><th>Designation</th><td>${emp.designation ?? ""}</td></tr>
                <tr><th>Department</th><td>${emp.department ?? ""}</td></tr>
                <tr><th>Employment Type</th><td>${emp.employmentType ?? ""}</td></tr>
                <tr><th>Skills</th><td>${emp.skills ?? ""}</td></tr>
                <tr><th>Salary</th><td>${emp.salary ?? ""}</td></tr>
                <tr><th>Payroll Cycle</th><td>${emp.payrollCycle ?? ""}</td></tr>
                

                <tr>
                    <th>Profile</th>
                    <td>${fileLink(emp.uploadProfile)}</td>
                </tr>
                <tr>
                    <th>Aadhaar</th>
                    <td>${fileLink(emp.aadhaarDocumentPath)}</td>
                </tr>
                <tr>
                    <th>PAN</th>
                    <td>${fileLink(emp.panDocumentPath)}</td>
                </tr>
                <tr>
                    <th>Agreement</th>
                    <td>${fileLink(emp.agreementPdfPath)}</td>
                </tr>
                <tr>
                    <th>Signature</th>
                    <td>${fileLink(emp.digitalSignaturePath)}</td>
                </tr>
            `;

                document.getElementById("viewEmployeeBody").innerHTML = html;

                new bootstrap.Modal(
                    document.getElementById("viewEmployeeModal")
                ).show();
            })
            .catch(err => alert(err.message));
    };

    function fileLink(path) {
        if (!path)
            return "<span class='text-muted'>Not uploaded</span>";

        return `
        <a href="${path}" target="_blank" class="btn btn-sm btn-primary">
            View
        </a>
        <a href="${path}" download class="btn btn-sm btn-success ms-2">
            Download
        </a>
    `;
    }


});


let canvas = null;
let ctx = null;
let drawing = false;

function initSignaturePad() {
    canvas = document.getElementById("signaturePad");
    if (!canvas) return;

    const parentWidth = canvas.parentElement.offsetWidth;

    // 🔥 SET REAL CANVAS SIZE (CRITICAL)
    canvas.width = parentWidth;
    canvas.height = 200;

    ctx = canvas.getContext("2d");
    ctx.strokeStyle = "#000";
    ctx.lineWidth = 2;
    ctx.lineCap = "round";

    drawing = false;

    // REMOVE old listeners
    canvas.replaceWith(canvas.cloneNode(true));
    canvas = document.getElementById("signaturePad");
    ctx = canvas.getContext("2d");

    // MOUSE EVENTS
    canvas.addEventListener("mousedown", startDraw);
    canvas.addEventListener("mousemove", draw);
    canvas.addEventListener("mouseup", stopDraw);
    canvas.addEventListener("mouseleave", stopDraw);

    // TOUCH EVENTS
    canvas.addEventListener("touchstart", startDraw, { passive: false });
    canvas.addEventListener("touchmove", draw, { passive: false });
    canvas.addEventListener("touchend", stopDraw);

    clearPad();
}

function startDraw(e) {
    e.preventDefault();
    drawing = true;

    const pos = getPosition(e);
    ctx.beginPath();
    ctx.moveTo(pos.x, pos.y);
}

function draw(e) {
    if (!drawing) return;
    e.preventDefault();

    const pos = getPosition(e);
    ctx.lineTo(pos.x, pos.y);
    ctx.stroke();
}


function stopDraw(e) {
    if (!drawing) return;
    drawing = false;

    const dataUrl = canvas.toDataURL("image/png");

    // ❗ Save base64
    document.getElementById("signatureData").value = dataUrl;

    // ❗ Clear file input if drawing used
    const fileInput = document.getElementById("signatureFileInput");
    fileInput.value = "";

    document.getElementById("signatureFileDisplay").innerText =
        "Digital signature captured";

    console.log("Signature saved ✔");
}

function getPosition(e) {
    const rect = canvas.getBoundingClientRect();

    let clientX, clientY;

    if (e.touches && e.touches.length > 0) {
        clientX = e.touches[0].clientX;
        clientY = e.touches[0].clientY;
    } else {
        clientX = e.clientX;
        clientY = e.clientY;
    }

    return {
        x: clientX - rect.left,
        y: clientY - rect.top
    };
}

function clearPad() {
    if (!ctx || !canvas) return;

    ctx.clearRect(0, 0, canvas.width, canvas.height);
    document.getElementById("signatureData").value = "";
}
