# Blackbird.io Figma

Blackbird is the new automation backbone for the language technology industry. Blackbird provides enterprise-scale automation and orchestration with a simple no-code/low-code platform. Blackbird enables ambitious organizations to identify, vet and automate as many processes as possible. Not just localization workflows, but any business and IT process. This repository represents an application that is deployable on Blackbird and usable inside the workflow editor.

## Introduction

<!-- begin docs -->

Figma is a cloud-based design and collaboration platform that specializes in user interface (UI) and user experience (UX) design. It provides a powerful, browser-based vector graphics editor that enables teams to create, prototype, and iterate on digital products in real time. Figma’s platform is known for its intuitive interface, seamless collaboration features, and ability to keep designers, developers, and stakeholders aligned throughout the entire product development process.

This App allows you to download and use Figma designs as images in Blackbird.

## Before setting up

Before you can connect you need to make sure that:

- You have a Figma account.
- You have a Personal access token for your Figma account. It can be found under Account settings > Security > Personal access tokens in Figma.
- You have an existing Project in Figma you want to connect to.

> Connecting to Figma with OAuth will be available soon.

## Connecting

1. Navigate to Apps, and find the Figma app. You can use search to find it.
2. Click _Add Connection_.
3. Name your connection for future reference e.g. 'My Figma connection'.
4. Fill in the Project ID of the project you want to connect to. You can find the Project ID in the URL when viewing a project: https://www.figma.com/design/:project_id/...
5. Fill in the Personal Access Token to your Figma account.
6. Click _Connect_.

## Actions

- **Download image** downloades an images of a specific canvas in your project. Multiple different formats are available (png, jpg, svg and pdf). If you want to get an image of a section of your canvas, then provide the node ID in the inputs.

> We are still developing the Figma App and want to hear your opinion! How do you ultimately want to translate content in Figma? What do you do today and how would you want to see that improved? If you're interested in using the Blackbird Figma App for translation then please reach out!

## Feedback

Do you want to use this app or do you have feedback on our implementation? Reach out to us using the [established channels](https://www.blackbird.io/) or create an issue.

<!-- end docs -->
