import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

version = "2022.04"

project {
    buildType(PeopleApiBuild)
}

object PeopleApiBuild : BuildType({
    name = "People API Build"

    vcs {
        root(DslContext.settingsRoot)
    }

    // ✅ Automatically trigger build on VCS changes
    triggers {
        vcs {
            // Optional: customize if needed
            // quietPeriodMode = VcsTrigger.QuietPeriodMode.USE_DEFAULT
            // branchFilter = "+:*"
        }
    }

    steps {
        script {
            name = "Restore, Build, Test"
            scriptContent = """
                cd src
                dotnet restore People.sln
                dotnet build People.sln --configuration Release
                dotnet test People.sln --configuration Release
            """.trimIndent()
        }
        script {
            name = "Build Docker Image"
            scriptContent = """
                cd ..
                getent group docker
                docker build -t localhost:5000/people-api:%build.number% -f Dockerfile .
            """.trimIndent()
        }

        script {
            name = "Push to Local Registry"
            scriptContent = """
                docker push localhost:5000/people-api:%build.number%
                docker inspect --format='{{index .RepoDigests 0}}' localhost:5000/people-api:%build.number% > image.digest
            """.trimIndent()
        }
    }

    artifactRules = "image.digest"
})
