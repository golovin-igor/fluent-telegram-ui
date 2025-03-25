# FluentTelegramUI Documentation

This directory contains the source files for the FluentTelegramUI documentation site, which is built using [GitHub Pages](https://pages.github.com/) and [Jekyll](https://jekyllrb.com/) with the [Just the Docs](https://just-the-docs.github.io/just-the-docs/) theme.

## Directory Structure

- `/docs/` - Root directory for all documentation
  - `/docs/getting-started/` - Installation and quick start guides
  - `/docs/components/` - Documentation for individual UI components
  - `/docs/advanced/` - Advanced topics and patterns
  - `/docs/examples/` - Example code and usage scenarios
  - `/docs/api/` - API reference documentation
  - `/docs/contributing/` - Guide for contributing to the documentation

## Contributing to Documentation

### Local Development

To develop documentation locally:

1. Install Ruby and Jekyll:
   ```bash
   gem install jekyll bundler
   ```

2. Navigate to the docs directory:
   ```bash
   cd docs
   ```

3. Install dependencies:
   ```bash
   bundle install
   ```

4. Run the local server:
   ```bash
   bundle exec jekyll serve
   ```

5. Open your browser to `http://localhost:4000/fluent-telegram-ui/` to see the site

### Adding New Pages

1. Create a new Markdown file in the appropriate directory
2. Include the following front matter at the top of the file:
   ```yaml
   ---
   layout: default
   title: Your Page Title
   parent: Parent Category
   nav_order: 1
   ---
   ```

3. Write your content using Markdown

### Documentation Guidelines

- Use clear, concise language
- Include code examples for important features
- Follow the formatting of existing pages
- Use appropriate heading levels (# for page title, ## for major sections, etc.)
- Include links to related documentation where appropriate
- Test all code examples to ensure they work as expected

## Building for Production

The documentation site is automatically built and deployed when changes are pushed to the main branch. You don't need to manually build it for production.

GitHub Pages will use the configuration in `_config.yml` to build the site.

## Notes

- The site URL is configured in `_config.yml`
- Navigation structure is determined by `parent` and `nav_order` in each file's front matter
- Images should be placed in the `/docs/assets/images/` directory 