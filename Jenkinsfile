def ckLib = library(
  identifier: 'ck_ip_jenkins_library@ck-phoenix-deployment-config',
  retriever: modernSCM([
    $class: 'GitSCMSource',
    remote: 'git@github.com:wiley/ck_ip_jenkins_library.git',
    credentialsId: 'babee6c1-14fe-4d90-9da0-ffa7068c69af'
  ])
)

phoenixDotnetPipeline(
  dockerImageName: 'phoenix-company-api',
  runTests: false,
  runATRegJob: [
    active: true,
    appSection: 'CompanyAPI',
    localEnvVar: 'COMPANY_API_IMAGE_TAG'
  ],
  namespacePrefix:'phoenix',
  artifacts: true,
  dockerFilePath: './CompanyAPI/Dockerfile'
)