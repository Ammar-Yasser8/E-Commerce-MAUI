import re
import os
from reportlab.lib.pagesizes import letter
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle, PageBreak, Image
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib import colors
from reportlab.pdfgen import canvas

# Highlight XML/XAML code for ReportLab's HTML-like parser
def highlight_xml_to_reportlab(code):
    # 1. Escape special characters for ReportLab XML parser
    code = code.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;').replace('"', '&quot;')
    
    # 2. Tokenize line by line to preserve spaces/indents
    lines = code.split('\n')
    highlighted_lines = []
    
    for line in lines:
        leading_spaces = len(line) - len(line.lstrip(' '))
        indent = '&nbsp;' * leading_spaces
        trimmed = line.lstrip(' ')
        
        # Color Comments (e.g. <!-- ... -->)
        if trimmed.startswith('&lt;!--') and trimmed.endswith('--&gt;'):
            highlighted = f'<font color="#6B7280">{trimmed}</font>'
        else:
            # Highlight Tag Name (e.g. &lt;/?TagName or &lt;TagName)
            highlighted = re.sub(r'(&lt;/?)([\w\:\-]+)', r'\1<font color="#0284C7"><b>\2</b></font>', trimmed)
            
            # Highlight closing brackets: (/?&gt;)
            highlighted = re.sub(r'(/?&gt;)', r'<font color="#0284C7"><b>\1</b></font>', highlighted)
            
            # Highlight attributes and values: attribute="value"
            highlighted = re.sub(r'([\w\:\-]+)=&quot;([^&quot;]*)&quot;', 
                                 r'<font color="#B45309">\1</font>=&quot;<font color="#059669">\2</font>&quot;', highlighted)
        
        highlighted_lines.append(indent + highlighted)
        
    return "<br/>".join(highlighted_lines)

# Custom Canvas for Header/Footer (Page numbering)
class NumberedCanvas(canvas.Canvas):
    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self._saved_page_states = []

    def showPage(self):
        self._saved_page_states.append(dict(self.__dict__))
        self._startPage()

    def save(self):
        num_pages = len(self._saved_page_states)
        for state in self._saved_page_states:
            self.__dict__.update(state)
            self.draw_page_number(num_pages)
            super().showPage()
        super().save()

    def draw_page_number(self, page_count):
        if self._pageNumber > 1:
            self.saveState()
            self.setFont("Helvetica-Oblique", 8)
            self.setFillColor(colors.HexColor("#6B7280"))
            
            # Header
            self.drawString(54, 750, "E-Commerce XAML Layout Documentation")
            self.drawRightString(612 - 54, 750, f"Page {self._pageNumber} of {page_count}")
            self.setStrokeColor(colors.HexColor("#E5E7EB"))
            self.setLineWidth(0.5)
            self.line(54, 742, 612 - 54, 742)
            
            # Footer
            self.line(54, 50, 612 - 54, 50)
            self.drawCentredString(612 / 2.0, 38, "Developed with .NET MAUI & ASP.NET Core API")
            self.restoreState()

# Load source files safely
src_dir = ""

def read_code_file(path, start=None, end=None):
    full_path = os.path.join(src_dir, path)
    if not os.path.exists(full_path):
        return f"[File not found: {path}]"
    try:
        with open(full_path, "r", encoding="utf-8") as f:
            lines = f.readlines()
        if start is not None and end is not None:
            return "".join(lines[start-1:end])
        return "".join(lines)
    except Exception as e:
        return f"[Error reading file: {e}]"

def build_pdf():
    pdf_path = os.path.join("docs", "E-Commerce_Documentation.pdf")
    doc = SimpleDocTemplate(
        pdf_path,
        pagesize=letter,
        leftMargin=54,
        rightMargin=54,
        topMargin=72,
        bottomMargin=72
    )
    
    styles = getSampleStyleSheet()
    
    # Custom styles
    title_style = ParagraphStyle(
        'DocTitle',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=28,
        leading=34,
        textColor=colors.HexColor("#0D9488"),
        alignment=1, # Center
        spaceAfter=15
    )
    
    subtitle_style = ParagraphStyle(
        'DocSubtitle',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=18,
        leading=22,
        textColor=colors.HexColor("#111827"),
        alignment=1,
        spaceAfter=10
    )
    
    info_style = ParagraphStyle(
        'DocInfo',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=11,
        leading=16,
        textColor=colors.HexColor("#4B5563"),
        alignment=1,
        spaceAfter=30
    )
    
    h1_style = ParagraphStyle(
        'H1',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=16,
        leading=20,
        textColor=colors.HexColor("#0D9488"),
        spaceBefore=15,
        spaceAfter=10
    )
    
    body_style = ParagraphStyle(
        'Body',
        parent=styles['Normal'],
        fontName='Helvetica',
        fontSize=10,
        leading=14,
        textColor=colors.HexColor("#374151"),
        spaceAfter=6
    )
    
    label_style = ParagraphStyle(
        'Label',
        parent=styles['Normal'],
        fontName='Helvetica-Bold',
        fontSize=10,
        leading=14,
        textColor=colors.HexColor("#111827")
    )
    
    code_style = ParagraphStyle(
        'CodeStyle',
        parent=styles['Normal'],
        fontName='Courier',
        fontSize=7.5,
        leading=9,
        textColor=colors.HexColor("#1F2937")
    )
 
    story = []
    
    # --- 1. COVER PAGE ---
    story.append(Spacer(1, 80))
    story.append(Paragraph("E-COMMERCE", title_style))
    story.append(Paragraph("XAML UI Layout Documentation", subtitle_style))
    story.append(Paragraph("Visual Screens & XML Structuring Reference", info_style))
    
    # Cover specs table
    specs_data = [
        [Paragraph("<b>Development Framework:</b>", body_style), Paragraph(".NET MAUI (XAML / C#)", body_style)],
        [Paragraph("<b>Architectural Pattern:</b>", body_style), Paragraph("MVVM (Model-View-ViewModel)", body_style)],
        [Paragraph("<b>Backend Integration:</b>", body_style), Paragraph("ASP.NET Core REST APIs", body_style)],
        [Paragraph("<b>Payment Gateway:</b>", body_style), Paragraph("Stripe Integration with Webhooks", body_style)]
    ]
    specs_table = Table(specs_data, colWidths=[160, 200])
    specs_table.setStyle(TableStyle([
        ('LINEBELOW', (0,0), (-1,-1), 0.5, colors.HexColor("#E5E7EB")),
        ('TOPPADDING', (0,0), (-1,-1), 8),
        ('BOTTOMPADDING', (0,0), (-1,-1), 8),
    ]))
    story.append(specs_table)
    story.append(PageBreak())
    
    # Helper to add a side-by-side page section (Left: Details + Code, Right: Image)
    def append_page_section_with_screenshot(title, desc, binding, xaml_path, xaml_start, xaml_end, screenshot_name):
        story.append(Paragraph(title, h1_style))
        
        # 1. Prepare Left Column content: Details and Code Box
        left_flowables = []
        left_flowables.append(Paragraph(f"<b>Description:</b> {desc}", body_style))
        left_flowables.append(Spacer(1, 4))
        left_flowables.append(Paragraph(f"<b>Key UI Bindings:</b><br/>{binding}", body_style))
        left_flowables.append(Spacer(1, 8))
        
        xaml_code = read_code_file(xaml_path, xaml_start, xaml_end)
        html_code = highlight_xml_to_reportlab(xaml_code)
        code_para = Paragraph(html_code, code_style)
        
        # We need a slightly narrower code box since it's sharing space with the image
        code_table = Table([[code_para]], colWidths=[314])
        code_table.setStyle(TableStyle([
            ('BACKGROUND', (0,0), (-1,-1), colors.HexColor("#F9FAFB")),
            ('LEFTPADDING', (0,0), (-1,-1), 8),
            ('RIGHTPADDING', (0,0), (-1,-1), 8),
            ('TOPPADDING', (0,0), (-1,-1), 8),
            ('BOTTOMPADDING', (0,0), (-1,-1), 8),
            ('BOX', (0,0), (-1,-1), 0.5, colors.HexColor("#E5E7EB")),
        ]))
        left_flowables.append(code_table)
        
        # 2. Prepare Right Column content: Screenshot Image
        right_flowables = []
        if screenshot_name:
            img_path = os.path.join(src_dir, "docs", "screenshots", screenshot_name)
            if os.path.exists(img_path):
                # Square screenshot (original is 1024x1024)
                img = Image(img_path, width=170, height=170)
                right_flowables.append(img)
            else:
                right_flowables.append(Paragraph(f"[Screenshot {screenshot_name} not found]", body_style))
        else:
            right_flowables.append(Paragraph("", body_style))
            
        # 3. Create two-column table layout
        col_table = Table([[left_flowables, right_flowables]], colWidths=[324, 180])
        col_table.setStyle(TableStyle([
            ('VALIGN', (0,0), (-1,-1), 'TOP'),
            ('ALIGN', (1,0), (1,0), 'CENTER'),
            ('LEFTPADDING', (0,0), (-1,-1), 0),
            ('RIGHTPADDING', (0,0), (-1,-1), 0),
            ('TOPPADDING', (0,0), (-1,-1), 0),
            ('BOTTOMPADDING', (0,0), (-1,-1), 0),
        ]))
        
        story.append(col_table)
        story.append(Spacer(1, 10))
        story.append(PageBreak())

    # --- 3. APP SHELL LAYOUT (Full width, no screenshot) ---
    append_page_section_with_screenshot(
        title="AppShell Navigation Structure",
        desc="Defines the primary app container, containing a bottom TabBar navigation schema. It sets the background colors, selected/unselected icon colors, and route points for core tabs.",
        binding="Tab 1 -> HomePage route (default page)<br/>Tab 2 -> CartPage route",
        xaml_path="AppShell.xaml",
        xaml_start=21,
        xaml_end=34,
        screenshot_name=None
    )

    # --- 4. LOGIN PAGE LAYOUT ---
    append_page_section_with_screenshot(
        title="1. LoginPage XAML Structure",
        desc="Defines the interface for user login. It contains an Error Message block that handles real-time warning indicators, input Entry containers for Email/Password, and the primary Action button.",
        binding="HasError -> Controls validation box visibility<br/>Email -> Entry field binding<br/>Password -> Password masking input<br/>LoginCommand -> Button trigger action",
        xaml_path="Views/LoginPage.xaml",
        xaml_start=65,
        xaml_end=74,
        screenshot_name="login_screen.png"
    )

    # --- 5. HOME PAGE LAYOUT ---
    append_page_section_with_screenshot(
        title="2. HomePage XAML Structure",
        desc="Features the main grid display. Highlights dynamic welcome labels, action icons (Cart, Settings), and a live searching framework. Displays categories as horizontal chips with active visual state triggers.",
        binding="UserName -> Displays personalized header<br/>SearchTerm -> Live search text entry<br/>Categories -> CollectionView of scrollable category chips<br/>ToggleCategoriesCommand -> Collapses/expands chips",
        xaml_path="Views/HomePage.xaml",
        xaml_start=111,
        xaml_end=125,
        screenshot_name="home_screen.png"
    )

    # --- 6. PRODUCT DETAILS LAYOUT ---
    append_page_section_with_screenshot(
        title="3. ProductDetailsPage XAML Structure",
        desc="Details the layout for single product items. Displays image, description, stock indicator, rating chips, and a sticky bottom grid holding the total price calculation and the primary cart action button.",
        binding="Product.Image -> Renders product image<br/>TotalPrice -> Real-time quantity-multiplied cost<br/>CartButtonText -> Dynamic CTA label ('Add to Cart' or 'Go to Cart')<br/>AddToCartCommand -> Submits data to CartService",
        xaml_path="Views/ProductDetailsPage.xaml",
        xaml_start=275,
        xaml_end=282,
        screenshot_name="product_details_screen.png"
    )

    # --- 7. CART PAGE LAYOUT ---
    append_page_section_with_screenshot(
        title="4. CartPage XAML Structure",
        desc="Shows list of items added to the cart inside a SwipeView container (which handles quick swiping deletion). It includes dynamic order totals (Subtotal, Shipping, and Final Price) at the bottom.",
        binding="CartItems -> List source bindings<br/>Subtotal / TotalAmount -> Cost summaries<br/>CheckoutCommand -> Submits orders to checkout flow<br/>RemoveItemCommand -> Triggers swiped item deletion",
        xaml_path="Views/CartPage.xaml",
        xaml_start=63,
        xaml_end=70,
        screenshot_name="cart_screen.png"
    )

    # Build Document
    doc.build(story, canvasmaker=NumberedCanvas)
    print(f"ReportLab PDF successfully generated at: {pdf_path}")

if __name__ == "__main__":
    build_pdf()
