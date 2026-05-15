<script setup lang="ts">
import { useHead } from '#imports'

useHead({
  title: 'Privacy Policy - Audio Atlas'
})
</script>

<template>
  <main class="legal-page">
    <div class="legal-container">
      <h1>Privacy Policy</h1>

      <p class="meta">
        Last updated: May 2026 · Effective: March 2026
      </p>

      <blockquote>
        The short version: We collect the minimum data needed to run Audio Atlas.
        We don't sell your data. You can delete your account and data at any time.
        We're based in Denmark and comply with GDPR.
      </blockquote>

      <!-- WHO WE ARE -->
      <section>
        <h2>1. Who We Are</h2>
        <p>
          Audio Atlas is an open-source platform for discovering and documenting
          music genres worldwide. It is developed and operated as a student project
          at the IT University of Copenhagen (ITU).
        </p>

        <p>
          <strong>Data controller:</strong> Audio Atlas Project Team,
          IT University of Copenhagen, Rued Langgaards Vej 7,
          2300 Copenhagen S, Denmark.
        </p>

        <p>
          For data protection queries, contact:
          <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a>
        </p>
      </section>

      <!-- DATA COLLECTION -->
      <section>
        <h2>2. What Data We Collect</h2>
        <p>
          We only collect data that is necessary for the platform to function.
          We do not collect any data beyond what is listed below.
        </p>

        <table>
          <thead>
            <tr>
              <th>Data</th>
              <th>Source</th>
              <th>Purpose</th>
              <th>Stored Until</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>Username</td>
              <td>You choose it at signup</td>
              <td>Display name on contributions</td>
              <td>Account deletion</td>
            </tr>
            <tr>
              <td>Email address</td>
              <td>OAuth provider (GitHub/Google)</td>
              <td>Account identification and contact</td>
              <td>Account deletion</td>
            </tr>
            <tr>
              <td>OAuth provider ID</td>
              <td>GitHub or Google</td>
              <td>Authentication (logging you in)</td>
              <td>Account deletion</td>
            </tr>
            <tr>
              <td>GDPR consent timestamp</td>
              <td>Generated at signup</td>
              <td>Recording your informed consent (legal requirement)</td>
              <td>Account deletion</td>
            </tr>
            <tr>
              <td>Contributed content</td>
              <td>You submit it</td>
              <td>Platform content (genre descriptions, sources, etc.)</td>
              <td>Anonymised on account deletion</td>
            </tr>
          </tbody>
        </table>

        <h3>What We Do NOT Collect</h3>
        <p>
          We do not collect or store: browsing behaviour, location data, IP addresses
          for tracking purposes, analytics or telemetry data, cookies beyond session
          authentication, or any data not listed in the table above. Profile picture
          URLs received from OAuth providers during login are not persisted to our database.
        </p>
      </section>

      <!-- LEGAL BASIS -->
      <section>
        <h2>3. Lawful Basis for Processing</h2>
        <p>Under GDPR Article 6, we process your personal data based on:</p>
        <ul>
          <li>
            <strong>Consent</strong> (Article 6(1)(a)): You explicitly agree to this
            policy and our contribution guidelines before your account is created.
            Your consent is recorded with a timestamp.
          </li>
          <li>
            <strong>Legitimate interest</strong> (Article 6(1)(f)): We maintain
            platform security and prevent abuse.
          </li>
        </ul>
      </section>

      <!-- DATA USAGE -->
      <section>
        <h2>4. How We Use Your Data</h2>
        <p>Your data is used solely to:</p>
        <ul>
          <li>Authenticate your account via OAuth</li>
          <li>Attribute your contributions to your chosen username</li>
          <li>Enable curators to manage submissions and communicate review outcomes</li>
          <li>Comply with legal obligations under GDPR</li>
        </ul>
        <p>
          We do not use your data for advertising, profiling, analytics, automated
          decision-making, or any purpose beyond operating the platform.
        </p>
      </section>

      <!-- GDPR RIGHTS -->
      <section>
        <h2>5. Your Rights Under GDPR</h2>
        <p>
          As a user of an EU-based platform, you have the following rights regardless
          of your location:
        </p>
        <ul>
          <li>
            <strong>Right of access</strong> (Article 15): Request a complete copy
            of all data we hold about you.
          </li>
          <li>
            <strong>Right to rectification</strong> (Article 16): Correct any
            inaccurate personal data.
          </li>
          <li>
            <strong>Right to erasure</strong> (Article 17): Delete your account and
            personal data at any time via your profile settings.
          </li>
          <li>
            <strong>Right to data portability</strong> (Article 20): Receive your
            personal data in a structured, machine-readable format.
          </li>
          <li>
            <strong>Right to restrict processing</strong> (Article 18): Request that
            we limit how your data is used.
          </li>
          <li>
            <strong>Right to object</strong> (Article 21): Object to processing of
            your data based on legitimate interest.
          </li>
        </ul>
        <p>
          To exercise any of these rights, use the account settings page or email
          <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a>.
          We will respond within 30 days.
        </p>
      </section>

      <!-- ACCOUNT DELETION -->
      <section>
        <h2>6. Account Deletion</h2>
        <p>
          You can delete your account at any time from your profile settings.
          When you delete your account:
        </p>
        <ul>
          <li>
            Your personal data (username, email, OAuth provider ID, consent record)
            is <strong>permanently and irreversibly removed</strong> from our database.
          </li>
          <li>
            Your contributed genre content <strong>remains on the platform</strong>,
            attributed to "Deleted User" rather than your username.
          </li>
          <li>
            Contributed content is retained because it is licensed under CC BY-SA 4.0
            and serves the public knowledge base. By submitting content, you grant
            this licence, which survives account deletion.
          </li>
          <li>
            Deletion is processed immediately. There is no recovery period or soft-delete.
          </li>
        </ul>
      </section>

      <!-- SECURITY -->
      <section>
        <h2>7. Data Storage and Security</h2>
        <ul>
          <li><strong>Database:</strong> Azure SQL Database hosted in EU data centres (West Europe region).</li>
          <li><strong>Hosting:</strong> Azure App Service (EU region).</li>
          <li><strong>Encryption in transit:</strong> All connections use HTTPS/TLS. HTTP requests are redirected to HTTPS.</li>
          <li><strong>Encryption at rest:</strong> Azure-managed encryption for database and backups (AES-256).</li>
          <li><strong>Access control:</strong> Production database access is restricted to platform administrators.</li>
          <li><strong>Source code:</strong> The platform codebase is open source (MIT licence) and does not contain personal data.</li>
        </ul>
      </section>

      <!-- COOKIES -->
      <section>
        <h2>8. Cookies</h2>
        <p>
          Audio Atlas uses <strong>only strictly necessary session cookies</strong>
          for authentication. These cookies:
        </p>
        <ul>
          <li>Identify your login session after OAuth authentication</li>
          <li>Are deleted when you log out or close your browser</li>
          <li>Cannot be used to track you across other websites</li>
        </ul>
        <p>
          We do not use tracking cookies, analytics cookies, advertising cookies, or
          any third-party cookies. Because we only use strictly necessary cookies,
          no cookie consent banner is required under the ePrivacy Directive.
        </p>
      </section>

      <!-- THIRD PARTIES -->
      <section>
        <h2>9. Third-Party Services</h2>
        <p>
          We share data only with the following third parties, solely for the purposes described:
        </p>

        <table>
          <thead>
            <tr>
              <th>Service</th>
              <th>Purpose</th>
              <th>Data Shared</th>
              <th>Privacy Policy</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>GitHub</td>
              <td>OAuth authentication</td>
              <td>Email address, display name (received during login only; not retained beyond what is listed in Section 2)</td>
              <td>
                <a
                  href="https://docs.github.com/en/site-policy/privacy-policies/github-general-privacy-statement"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  github.com/privacy
                </a>
              </td>
            </tr>
            <tr>
              <td>Google</td>
              <td>OAuth authentication</td>
              <td>Email address, display name, profile picture URL (received during login only; profile picture URL is not stored)</td>
              <td>
                <a
                  href="https://policies.google.com/privacy"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  policies.google.com/privacy
                </a>
              </td>
            </tr>
            <tr>
              <td>Microsoft Azure</td>
              <td>Hosting and database</td>
              <td>All platform data (stored in EU)</td>
              <td>
                <a
                  href="https://privacy.microsoft.com/en-us/privacystatement"
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  privacy.microsoft.com
                </a>
              </td>
            </tr>
          </tbody>
        </table>

        <p>
          We do not sell, trade, rent, or otherwise share your personal data with
          any other third party.
        </p>
      </section>

      <!-- GOOGLE API SERVICES -->
      <section>
        <h2>10. Google API Services</h2>
        <p>
          This section specifically describes how Audio Atlas accesses, uses, stores,
          and protects data received from Google when you choose to sign in with your
          Google account. It is provided in accordance with the
          <a
            href="https://developers.google.com/terms/api-services-user-data-policy"
            target="_blank"
            rel="noopener noreferrer"
          >
            Google API Services User Data Policy
          </a>.
        </p>

        <h3>Data We Access from Google</h3>
        <p>
          When you sign in with Google, we request access only to the following data
          from your Google account:
        </p>
        <ul>
          <li>Your Google account email address</li>
          <li>Your display name</li>
          <li>Your Google profile picture URL</li>
        </ul>
        <p>
          We do not request access to Google Drive, Gmail, Google Contacts, Google
          Calendar, or any other Google service. Our use of Google account data is
          limited strictly to the scopes listed above.
        </p>

        <h3>How We Use Google Account Data</h3>
        <p>We use your Google account data solely to:</p>
        <ul>
          <li>Create and uniquely identify your Audio Atlas account</li>
          <li>Pre-fill your display name during account setup</li>
          <li>Display your name within the platform (for example, on your contributor profile)</li>
        </ul>
        <p>
          We do not use your Google account data to build advertising profiles,
          conduct behavioural tracking, or for any purpose beyond authenticating
          your identity and operating your Audio Atlas account. Google account data
          is never used for automated decision-making.
        </p>

        <h3>Data We Do Not Retain from Google</h3>
        <p>
          Your Google profile picture URL is received transiently during the OAuth
          login flow and is <strong>not stored</strong> in our database. We store
          only your email address and OAuth provider ID (see Section 2).
        </p>

        <h3>Data Sharing</h3>
        <p>
          We do not sell, rent, or share your Google account data with third parties.
          Your data is not disclosed to any external parties except where required
          by law, or where strictly necessary to operate the platform. Microsoft Azure,
          our hosting and database provider, processes data on our behalf under a
          data processing agreement and is bound by appropriate data protection obligations.
        </p>

        <h3>Data Retention and Deletion</h3>
        <p>
          Your Google account data is retained for as long as your Audio Atlas account
          remains active. You may delete your Audio Atlas account at any time from your
          account settings page. Upon account deletion:
        </p>
        <ul>
          <li>
            Your email address and OAuth provider ID are permanently and irreversibly
            removed from our database.
          </li>
          <li>
            Any content you contributed is anonymised and dissociated from your identity,
            in order to preserve the integrity of the platform's collaborative dataset.
          </li>
          <li>Deletion is processed immediately upon confirmation.</li>
        </ul>
        <p>
          To request deletion of your data, you may also contact us directly at
          <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a>.
        </p>

        <h3>Compliance Statement</h3>
        <p>
          Audio Atlas's use of information received from Google APIs adheres to the
          <a
            href="https://developers.google.com/terms/api-services-user-data-policy"
            target="_blank"
            rel="noopener noreferrer"
          >
            Google API Services User Data Policy
          </a>,
          including the Limited Use requirements. We do not transfer, use, or sell
          Google user data for serving advertisements; we do not allow humans to read
          Google user data unless we have your affirmative consent, it is necessary
          for security purposes, or it is required by law.
        </p>
      </section>

      <!-- BREACH -->
      <section>
        <h2>11. Data Breach Notification</h2>
        <p>In the event of a personal data breach, we will:</p>
        <ul>
          <li>
            Notify the Danish Data Protection Agency (Datatilsynet) within 72 hours
            of discovery, where required under GDPR Article 33.
          </li>
          <li>
            Notify affected users without undue delay if the breach is likely to
            result in a high risk to their rights and freedoms, as required under
            GDPR Article 34.
          </li>
        </ul>
      </section>

      <!-- CHILDREN -->
      <section>
        <h2>12. Children's Privacy</h2>
        <p>
          Audio Atlas is not directed at children under the age of 16. We do not
          knowingly collect personal data from children under 16. If we become aware
          that we have collected data from a child under 16 without parental consent,
          we will delete that data promptly.
        </p>
      </section>

      <!-- TRANSFERS -->
      <section>
        <h2>13. International Data Transfers</h2>
        <p>
          All data is stored and processed within the European Economic Area (EEA).
          We do not transfer personal data outside the EEA. OAuth authentication with
          GitHub and Google involves communication with servers that may be located
          outside the EEA; these providers operate under Standard Contractual Clauses
          approved by the European Commission.
        </p>
      </section>

      <!-- CHANGES -->
      <section>
        <h2>14. Changes to This Policy</h2>
        <p>
          We may update this privacy policy to reflect changes in our data practices
          or legal requirements. When we make changes:
        </p>
        <ul>
          <li>The "Last updated" date at the top of this page will be revised.</li>
          <li>Significant changes will be communicated to registered users via a notice on the platform.</li>
          <li>Continued use of the platform after changes constitutes acceptance of the revised policy.</li>
        </ul>
      </section>

      <!-- COMPLAINTS -->
      <section>
        <h2>15. Complaints</h2>
        <p>
          If you believe your data protection rights have been violated, you can
          contact us at
          <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a>.
          You also have the right to lodge a complaint with the Danish Data Protection Agency:
        </p>
        <p>
          <strong>Datatilsynet</strong><br>
          Carl Jacobsens Vej 35, 2500 Valby, Denmark<br>
          Website:
          <a
            href="https://www.datatilsynet.dk"
            target="_blank"
            rel="noopener noreferrer"
          >
            datatilsynet.dk
          </a><br>
          Email: <a href="mailto:dt@datatilsynet.dk">dt@datatilsynet.dk</a>
        </p>
      </section>

      <!-- CONTACT -->
      <section>
        <h2>16. Contact</h2>
        <p>For any questions about this privacy policy or your personal data:</p>
        <p><strong>Email:</strong> <a href="mailto:projectaudioatlas@gmail.com">projectaudioatlas@gmail.com</a></p>
        <p><strong>Project team:</strong> Audio Atlas, IT University of Copenhagen</p>
        <p>
          <strong>GitHub:</strong>
          <a
            href="https://github.com/AudioAtlas-xyz/Audio-Atlas"
            target="_blank"
            rel="noopener noreferrer"
          >
            Audio-Atlas
          </a>
        </p>
      </section>
    </div>
  </main>
</template>

<style scoped>
.legal-page {
  min-height: 100vh;
  background: #02070a;
  color: #eef2ff;
  padding: 4rem 1.5rem;
}

.legal-container {
  max-width: 900px;
  margin: 0 auto;
  padding: 2.5rem;
  background: #050816;
  border: 1px solid rgba(120, 150, 255, 0.18);
  border-radius: 24px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.45);
}

h1 {
  margin-top: 0;
  font-size: 2.5rem;
}

.meta {
  color: #8a92b8;
  margin-bottom: 2rem;
}

blockquote {
  margin: 0 0 2rem;
  padding: 1rem 1.25rem;
  border-left: 3px solid #44f0c4;
  background: rgba(68, 240, 196, 0.08);
  color: #d6fff5;
}

section {
  margin-top: 2rem;
}

h2 {
  color: #44f0c4;
  margin-bottom: 0.75rem;
}

h3 {
  margin-top: 1.5rem;
  color: #eef2ff;
}

p,
li {
  line-height: 1.7;
  color: #d7ddff;
}

ul {
  padding-left: 1.5rem;
}

a {
  color: #8b8dff;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 1rem;
}

th,
td {
  border: 1px solid rgba(120, 150, 255, 0.18);
  padding: 0.75rem;
  text-align: left;
}

th {
  background: rgba(120, 150, 255, 0.08);
}
</style>
