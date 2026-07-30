# Blackbird.io Figma

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Figma is a cloud-based design and collaboration platform that specializes in user interface (UI) and user experience (UX) design. It provides a powerful, browser-based vector graphics editor that enables teams to create, prototype, and iterate on digital products in real time. Figma’s platform is known for its intuitive interface, seamless collaboration features, and ability to keep designers, developers, and stakeholders aligned throughout the entire product development process.

Use this app to download images and variables from your Figma files, then upload translated variables.

> All Figma plans have access to the **Download image** Action. However, in order to use Figma variables with Blackbird you need to be on the Enterprise plan. See: [the Figma developer docs for variables](https://developers.figma.com/docs/rest-api/variables-endpoints/#get-local-variables-endpoint)

## Before setting up

Before you can connect, make sure that:

- You have a Figma account.
- You have a Personal Access Token for your Figma account. You can find it under **Account settings** > **Security** > **Personal access tokens**. Make sure your access token has the following scopes: *projects:read, file_content:read, file_metadata:read, file_variables:read, file_variables:write*
- You have an existing Figma project you want to connect to.

> Connecting to Figma with OAuth will be available soon.

## Connecting

1. Navigate to Apps and find the Figma app. You can use Search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference, for example, 'My Figma connection'.
4. Enter the Project ID for the project you want to connect to. You can find it in the URL when viewing the project: https://www.figma.com/design/:project_id/...
5. Enter the Personal Access Token for your Figma account.
6. Click _Connect_.

## Actions

### Variables

- **Download variables** Download variables from a Figma project.
- **Upload variables** Upload translated variables to a Figma project.
    Advanced settings:
  - **Target mode**: The mode to upload the variables to. If omitted, the language in the uploaded file is used.

### Images

- **Download image** Download an image from a file.
    Advanced settings:
  - **Format**: The image format to download. If omitted, PNG is used.

## Events

### Variables

- **On variables updated** Triggers when variables in a mode are updated.
    Advanced settings:
  - **Modes**: The modes to consider. If omitted, all modes are checked.

## Localizing Figma variables

[Figma variables](https://help.figma.com/hc/en-us/articles/15339657135383-Guide-to-variables-in-Figma) are reusable values that you can apply to design properties, including text fields. For localization, create at least one collection that contains text variables and bind those variables to the text in your designs.

Treat the collection as a table of localizable copy: each row is one uniquely named variable, and each mode is a language. For example, create an `en-us` mode for source copy and add an `es-es` mode for Spanish. Each variable then has a value for every language, and changing the selected mode lets you preview the localized text in the same design.

Figma users can add variables as their designs evolve. When a Bird is configured to download and translate the collection, new variables are included automatically and translated values can be uploaded to the appropriate language mode. The same structure can also support automated workflows that synchronize Figma variables with product translation keys in developer repositories.

> We are still developing the Figma app and want to hear your opinion. How do you ultimately want to translate content in Figma? What do you do today, and how would you want to see that improved? If you're interested in using the Blackbird Figma app for translation, please reach out.

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
