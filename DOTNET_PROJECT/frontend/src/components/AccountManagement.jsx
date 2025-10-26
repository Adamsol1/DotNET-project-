import React, { useState } from "react";

// <-- TODO: Does app use tokens? If so, adjust getAuthHeader() accordingly -->
// <-- TODO: Adjust validation rules/messages -->
// <-- TODO: Adjust API endpoints -->
// <-- TODO: Comment the code -->
// <-- TODO: Fix styling -->

export default function AccountManagement() {
    const [username, setUsername] = useState("");
    const [usernameMsg, setUsernameMsg] = useState(null);
    const [usernameLoading, setUsernameLoading] = useState(false);

    const [currentPassword, setCurrentPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [passwordMsg, setPasswordMsg] = useState(null);
    const [passwordLoading, setPasswordLoading] = useState(false);

    const [deleteLoading, setDeleteLoading] = useState(false);
    const [deleteMsg, setDeleteMsg] = useState(null);

    function getAuthHeader() {
        const token = localStorage.getItem("token");
        return token ? { Authorization: `Bearer ${token}` } : {};
    }

    async function handleUpdateUsername(e) {
        e.preventDefault();
        setUsernameMsg(null);

        if (!username || username.trim().length < 3) {
            setUsernameMsg({ type: "error", text: "Username must be at least 3 characters." });
            return;
        }

        setUsernameLoading(true);
        try {
            // TODO: Adjust endpoint
            const res = await fetch("", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    ...getAuthHeader(),
                },
                body: JSON.stringify({ username: username.trim() }),
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({ message: "Failed to update username" }));
                throw new Error(err.message || "Failed to update username");
            }

            setUsernameMsg({ type: "success", text: "Username updated." });
        } catch (err) {
            setUsernameMsg({ type: "error", text: err.message });
        } finally {
            setUsernameLoading(false);
        }
    }

    async function handleUpdatePassword(e) {
        e.preventDefault();
        setPasswordMsg(null);

        if (!currentPassword) {
            setPasswordMsg({ type: "error", text: "Current password is required." });
            return;
        }
        if (!newPassword || newPassword.length < 8) {
            setPasswordMsg({ type: "error", text: "New password must be at least 8 characters." });
            return;
        }
        if (newPassword !== confirmPassword) {
            setPasswordMsg({ type: "error", text: "New password and confirmation do not match." });
            return;
        }

        setPasswordLoading(true);
        try {
            // TODO: Adjust endpoint
            const res = await fetch("", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    ...getAuthHeader(),
                },
                body: JSON.stringify({ currentPassword, newPassword }),
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({ message: "Failed to update password" }));
                throw new Error(err.message || "Failed to update password");
            }

            setPasswordMsg({ type: "success", text: "Password updated." });
            setCurrentPassword("");
            setNewPassword("");
            setConfirmPassword("");
        } catch (err) {
            setPasswordMsg({ type: "error", text: err.message });
        } finally {
            setPasswordLoading(false);
        }
    }

    async function handleDeleteAccount() {
        setDeleteMsg(null);
        setDeleteLoading(true);
        try {
            // TODO: Adjust endpoint
            const res = await fetch("", {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                    ...getAuthHeader(),
                },
            });

            if (!res.ok) {
                const err = await res.json().catch(() => ({ message: "Failed to delete account" }));
                throw new Error(err.message || "Failed to delete account");
            }

            // On success, clear auth and redirect to home/login
            localStorage.removeItem("token");
            setDeleteMsg({ type: "success", text: "Account deleted. Redirecting..." });
            setTimeout(() => {
                window.location.href = "/"; // change to login route if desired
            }, 1200);
        } catch (err) {
            setDeleteMsg({ type: "error", text: err.message });
        } finally {
            setDeleteLoading(false);
        }
    }

    return (
        <div style={{ maxWidth: 720, margin: "24px auto", padding: 16 }}>
            <h2>Account Management</h2>

            <section style={{ border: "1px solid #ddd", padding: 16, marginBottom: 16 }}>
                <h3>Update Username</h3>
                <form onSubmit={handleUpdateUsername}>
                    <div style={{ marginBottom: 8 }}>
                        <label>
                            New username
                            <input
                                type="text"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                                placeholder="Enter new username"
                            />
                        </label>
                    </div>
                    <button type="submit" disabled={usernameLoading}>
                        {usernameLoading ? "Saving..." : "Save username"}
                    </button>
                </form>
                {usernameMsg && (
                    <div style={{ marginTop: 8, color: usernameMsg.type === "error" ? "red" : "green" }}>
                        {usernameMsg.text}
                    </div>
                )}
            </section>

            <section style={{ border: "1px solid #ddd", padding: 16, marginBottom: 16 }}>
                <h3>Update Password</h3>
                <form onSubmit={handleUpdatePassword}>
                    <div style={{ marginBottom: 8 }}>
                        <label>
                            Current password
                            <input
                                type="password"
                                value={currentPassword}
                                onChange={(e) => setCurrentPassword(e.target.value)}
                                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                                placeholder="Current password"
                            />
                        </label>
                    </div>

                    <div style={{ marginBottom: 8 }}>
                        <label>
                            New password
                            <input
                                type="password"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                                placeholder="New password (min 8 chars)"
                            />
                        </label>
                    </div>

                    <div style={{ marginBottom: 8 }}>
                        <label>
                            Confirm new password
                            <input
                                type="password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                style={{ display: "block", width: "100%", padding: 8, marginTop: 4 }}
                                placeholder="Confirm new password"
                            />
                        </label>
                    </div>

                    <button type="submit" disabled={passwordLoading}>
                        {passwordLoading ? "Saving..." : "Save password"}
                    </button>
                </form>
                {passwordMsg && (
                    <div style={{ marginTop: 8, color: passwordMsg.type === "error" ? "red" : "green" }}>
                        {passwordMsg.text}
                    </div>
                )}
            </section>

            <section style={{ border: "1px solid #f5c6cb", padding: 16, background: "#fff5f5" }}>
                <h3 style={{ color: "#a71d2a" }}>Delete Account</h3>
                <p>This action is permanent. All your data will be removed.</p>
                <button
                    onClick={handleDeleteAccount}
                    disabled={deleteLoading}
                    style={{ background: "#a71d2a", color: "#fff", padding: "8px 12px", border: "none" }}
                >
                    {deleteLoading ? "Deleting..." : "Delete account"}
                </button>
                {deleteMsg && (
                    <div style={{ marginTop: 8, color: deleteMsg.type === "error" ? "red" : "green" }}>
                        {deleteMsg.text}
                    </div>
                )}
            </section>
        </div>
    );
}
